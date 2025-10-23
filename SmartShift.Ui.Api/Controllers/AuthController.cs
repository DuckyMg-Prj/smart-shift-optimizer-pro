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
        [HttpPost, Route("register")]
        public IHttpActionResult Register(RegisterDto dto)
        {
            try
            {
                var user = authService.Register(dto);
                return Ok(new { Level = MessegeResource.Api_Common_Success, message = MessegeResource.sAuth_Register_Success, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(MessegeResource.Api_Common_Error + ": " + ex.Message);
            }
        }

        [HttpPost, Route("login")]
        public IHttpActionResult Login(LoginDto dto)
        {
            try
            {
                var token = authService.Login(dto);
                return Ok(new { Level = MessegeResource.Api_Common_Success, message = "Token: " + token });
            }
            catch (Exception ex)
            {
                return BadRequest(MessegeResource.Api_Common_Error + ": " + ex.Message);
            }
        }
    }
}
