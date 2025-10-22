using System;

namespace SmartShift.Core.Model.Entities
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InviteCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}