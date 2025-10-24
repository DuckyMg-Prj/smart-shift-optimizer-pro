
using SmartShift.Core.Model.DTOs.Report;
using System;

namespace SmartShift.Core.Service.Abstraction
{
    public interface IShiftReportService
    {
        CompanyReportDto GetCompanyReport(int companyId, DateTime fromUtc, DateTime toUtc);
        EmployeeReportDto GetEmployeeReport(int userId, DateTime fromUtc, DateTime toUtc);
    }
}