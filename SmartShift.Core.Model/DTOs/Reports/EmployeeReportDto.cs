

using SmartShift.Core.Model.DTOs.Users.Shift;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartShift.Core.Model.DTOs.Report
{
    public class EmployeeReportDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public double TotalHours { get; set; }
        public double OvertimeHours { get; set; }
        public int OffDays { get; set; }
    }
}