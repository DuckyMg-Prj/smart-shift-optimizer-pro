using System;
namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftAssignmentDto
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool IsConfirmed { get; set; }

    }
}