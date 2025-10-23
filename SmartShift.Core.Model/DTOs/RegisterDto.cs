using SmartShift.Core.Model.Enums;

namespace SmartShift.Core.Model.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleTypeKind Role { get; set; }  
        public string CompanyName { get; set; } 
    }
}