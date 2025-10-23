using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using SmartShift.Core.Data.Context;
using SmartShift.Core.Model;
using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.Entities;
using SmartShift.Core.Model.Enums;
using SmartShift.Core.Model.Source;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Core.Service.Resourcess;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SmartShift.Core.Service
{
    public class AuthService : IAuthService
    {
        private readonly Lazy<AppDbContext> _db;
        private readonly string _jwtSecret = "kA93@x2!pZ8rG1hFq6nVbT4uW9eLcY7z"; //config
        ///private readonly int _jwtExpireDays = int.Parse(ConfigurationManager.AppSettings["JwtExpireDays"]);

        public AuthService(Lazy<AppDbContext> db)
        {
            _db = db;
        }
        private AppDbContext db => _db.Value;
        public string GenerateJwtToken(UserModel user)
        {
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role.ToString())
            }),

                Expires = DateTime.UtcNow.AddMinutes(30) ,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public RefreshToken CreateRefreshToken(int userId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var refresh = new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            db.RefreshTokens.Add(refresh);
            db.SaveChanges();
            return refresh;
        }

        public RefreshToken ValidateRefreshToken(string token)
        {
            var refresh = db.RefreshTokens
                .SingleOrDefault(r => r.Token == token && r.RevokedAt == null);

            if (refresh == null || refresh.ExpiresAt < DateTime.UtcNow)
                return null;

            return refresh;
        }

        public RefreshToken RotateRefreshToken(RefreshToken old, string clientId)
        {
            old.RevokedAt = DateTime.UtcNow;
            var newRefresh = CreateRefreshToken(old.UserId);
            old.ReplacedByToken = newRefresh.Token;
            db.SaveChanges();
            return newRefresh;
        }
        public UserModel ValidateCredentials(string email, string password)
        {
            var user = db.UserModel.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                throw new CustomException(ExceptionResources.sAuthService_Login_EmailNotExist, ExceptionLevel.Warning);
            }

            if (user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value > DateTime.UtcNow)
                throw new CustomException($"Account locked until {user.LockoutEndUtc.Value:u}", ExceptionLevel.Warning);

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash, true, HashType.SHA256))
            {
                user.FailedLoginCount++;
                user.LastFailedLoginUtc = DateTime.UtcNow;
                if (user.FailedLoginCount >= 3)
                {
                    user.LockoutEndUtc = DateTime.UtcNow.AddMinutes(15);
                    // optionally log an event / send email
                }
                db.SaveChanges();

                throw new CustomException(ExceptionResources.sAuthService_Login_PasswordIsIncorrect, ExceptionLevel.Warning);
            }
            // success:
            user.FailedLoginCount = 0;
            user.LockoutEndUtc = null;
            db.SaveChanges();
            return user;
        }

        public UserModel Register(RegisterDto dto)
        {
            var query = from x in db.UserModel
                        select x;


            if (query != null && query.Any(u => u.Email == dto.Email))
            {
                throw new CustomException(ExceptionResources.sAuthService_Register_EmailAlreadyExists, ExceptionLevel.Warning);
            }


            if (dto.Role == RoleTypeKind.None)
            {
                throw new CustomException(ExceptionResources.sAuthService_Register_InvalidUserRole, ExceptionLevel.Error);
            }

            var hashed = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, HashType.SHA256);


            var user = new UserModel
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashed,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            db.UserModel.Add(user);
            db.SaveChanges();

            // if employer, create company
            if (dto.Role == RoleTypeKind.Employee && !string.IsNullOrEmpty(dto.CompanyName))
            {
                var company = new CompanyModel
                {
                    Name = dto.CompanyName,
                    InviteCode = Guid.NewGuid().ToString().Substring(0, 8),
                    CreatedBy = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                db.CompanyModel.Add(company);
                db.SaveChanges();

                user.CompanyId = company.Id;
                db.SaveChanges();
            }

            return user;
        }

        public string Login(LoginDto dto)
        {

            var user = ValidateCredentials(dto.Email, dto.Password);

            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role.ToString())
            }),

                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
