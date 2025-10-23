using SmartShift.Core.Model.Enums;
using System;

namespace SmartShift.Core.Model.Entities
{
    public class UserModel
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public RoleTypeKind Role { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int? MaxHoursPerWeek { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int FailedLoginCount { get; set; } = 0;
        public DateTime? LockoutEndUtc { get; set; } = null;
        public DateTime? LastFailedLoginUtc { get; set; }
    }
}