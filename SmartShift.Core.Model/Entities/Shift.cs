using System;

namespace SmartShift.Core.Model.Entities
{
    public class Shift
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }  
        public int? OwnerUserId { get; set; }    
        public DateTime StartUtc { get; set; } 
        public DateTime EndUtc { get; set; }
        public int RequiredStaff { get; set; } = 1;
        public bool IsOffDay { get; set; } = false; 
        public int? ClientId { get; set; }     
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
