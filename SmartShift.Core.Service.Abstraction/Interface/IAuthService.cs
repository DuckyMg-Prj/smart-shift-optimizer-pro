using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Users;
using SmartShift.Core.Model.Entities;

namespace SmartShift.Core.Service.Abstraction
{
    public interface IAuthService
    {
        UserModel Register(RegisterDto dto);
        string Login(LoginDto dto);
        UserModel ValidateCredentials(string email, string password);
        string GenerateJwtToken(UserModel user);
        RefreshToken CreateRefreshToken(int userId);
        RefreshToken ValidateRefreshToken(string refreshToken);
        RefreshToken RotateRefreshToken(RefreshToken oldToken, string clientId);
    }
}