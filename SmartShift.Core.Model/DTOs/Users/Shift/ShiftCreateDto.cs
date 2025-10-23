using System;
namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftCreateDto
    {
        public int? CompanyId { get; set; }
        public DateTime StartLocal { get; set; }
        public DateTime EndLocal { get; set; }
        public int RequiredStaff { get; set; } = 1;
        public bool IsOffDay { get; set; } = false;
        public int? ClientId { get; set; }
    }
}