using System;

namespace SmartShift.Core.Model.Entities
{
    public enum ShiftRequestType
    {
        Swap,
        Cancel,
        Unavailable
    }

    public enum ShiftRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class ShiftRequest
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int RequestedByUserId { get; set; }
        public int? TargetUserId { get; set; } // for swap target
        public ShiftRequestType Type { get; set; }
        public ShiftRequestStatus Status { get; set; } = ShiftRequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
