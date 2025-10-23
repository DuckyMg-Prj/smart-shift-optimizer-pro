using SmartShift.Core.Model.Enums;

namespace SmartShift.Core.Model.DTOs.Users
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public RoleTypeKind Role { get; set; }
        public int? CompanyId { get; set; }
    }
}
