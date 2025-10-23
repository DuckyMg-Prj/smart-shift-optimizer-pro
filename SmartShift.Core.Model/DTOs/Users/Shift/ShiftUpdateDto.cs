using System;

namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftUpdateDto
    {
        public DateTime StartLocal { get; set; }
        public DateTime EndLocal { get; set; }
        public int RequiredStaff { get; set; }
        public bool IsOffDay { get; set; }
    }
}