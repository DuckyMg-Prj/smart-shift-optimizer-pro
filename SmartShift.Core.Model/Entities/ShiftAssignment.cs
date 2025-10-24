using System;

namespace SmartShift.Core.Model.Entities
{

    public class ShiftAssignment
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsConfirmed { get; set; } = true;   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
