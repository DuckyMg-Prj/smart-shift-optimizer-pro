using System;

namespace SmartShift.Core.Model.DTOs.Users.Shift
{
    public class ShiftRequestDto
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int RequestedByUserId { get; set; }
        public string RequestedByName { get; set; }
        public int? TargetUserId { get; set; }
        public string TargetUserName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }


}