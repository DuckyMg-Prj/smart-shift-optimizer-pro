using System;
namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftSummaryDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TotalShifts { get; set; }
        public double TotalHours { get; set; }
        public double OvertimeHours { get; set; }
        public int OffDays { get; set; }
    }
}