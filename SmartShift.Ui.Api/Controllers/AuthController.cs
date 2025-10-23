using System;
using System.Web.Http;
using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Ui.Api.Resourcess;
namespace SmartShift.Ui.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly Lazy<IAuthService> _authService;
        public AuthController(Lazy<IAuthService> authService)
        {
            _authService = authService;
        }


        private IAuthService authService => _authService.Value;

        [HttpPost, Route("token")]
        public IHttpActionResult Token(TokenRequest dto)
        {
            if (dto.GrantType == "password")
            {
                var user = authService.ValidateCredentials(dto.Username, dto.Password);
                if (user == null)
                    return BadRequest("Invalid credentials");

                var accessToken = authService.GenerateJwtToken(user);
                var refresh = authService.CreateRefreshToken(user.Id);

                return Ok(new
                {
                    access_token = accessToken,
                    token_type = "bearer",
                    expires_in = 3600,
                    refresh_token = refresh.Token
                });
            }
            else if (dto.GrantType == "refresh_token")
            {
                var refresh = authService.ValidateRefreshToken(dto.RefreshToken);
                if (refresh == null)
                    return BadRequest("invalid_grant");

                var user = authService.ValidateCredentials(dto.Username, dto.Password);
                var accessToken = authService.GenerateJwtToken(user);
                var newRefresh = authService.RotateRefreshToken(refresh, dto.ClientId);

                return Ok(new
                {
                    access_token = accessToken,
                    token_type = "bearer",
                    expires_in = 3600,
                    refresh_token = newRefresh.Token
                });
            }

            return BadRequest("unsupported_grant_type");
        }


        [HttpPost, Route("register")]
        public IHttpActionResult Register(RegisterDto dto)
        {
            try
            {
                var user = authService.Register(dto);
                return Ok(new { Level = MessageResources.Api_Common_Success, message = MessageResources.cAuth_Register_Success, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(MessageResources.Api_Common_Error + ": " + ex.Message);
            }
        }

        [HttpPost, Route("login")]
        public IHttpActionResult Login(LoginDto dto)
        {
            try
            {
                var token = authService.Login(dto);
                return Ok(new { Level = MessageResources.Api_Common_Success, message = "Token: " + token });
            }
            catch (Exception ex)
            {
                return BadRequest(MessageResources.Api_Common_Error + ": " + ex.Message);
            }
        }
    }
}
