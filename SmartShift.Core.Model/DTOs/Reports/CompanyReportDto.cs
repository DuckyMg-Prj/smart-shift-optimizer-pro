

using SmartShift.Core.Model.DTOs.Users.Shift;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartShift.Core.Model.DTOs.Report
{
    public class CompanyReportDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public List<ShiftSummaryDto> EmployeeSummaries { get; set; } = new List<ShiftSummaryDto>();
        public double TotalCompanyHours => EmployeeSummaries.Sum(e => e.TotalHours);
    }
}