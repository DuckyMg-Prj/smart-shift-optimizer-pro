using System;
namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftRequestCreateDto
    {
        public int ShiftId { get; set; }
        public int? TargetUserId { get; set; }
        public string Type { get; set; }
    }

}