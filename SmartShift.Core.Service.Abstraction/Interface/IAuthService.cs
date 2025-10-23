using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.Entities;

namespace SmartShift.Core.Service.Abstraction
{
    public interface IAuthService
    {
        UserModel Register(RegisterDto dto);
        string Login(LoginDto dto);
    }
}