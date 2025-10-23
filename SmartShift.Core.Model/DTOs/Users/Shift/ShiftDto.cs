using System;
namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftDto
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public int? OwnerUserId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int RequiredStaff { get; set; }
        public bool IsOffDay { get; set; }
    }
}