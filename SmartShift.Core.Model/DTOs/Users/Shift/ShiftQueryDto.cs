using System;

namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftQueryDto
    {
        public int? CompanyId { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
    }

}