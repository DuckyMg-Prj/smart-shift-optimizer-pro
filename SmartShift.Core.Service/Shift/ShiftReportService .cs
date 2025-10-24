using SmartShift.Core.Data.Context;
using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Report;
using SmartShift.Core.Model.DTOs.Users.Shift;
using SmartShift.Core.Model.Entities;
using SmartShift.Core.Model.Enums;
using SmartShift.Core.Model.Source;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Core.Service.Resourcess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartShift.Core.Service.shift
{

    public class ShiftReportService : IShiftReportService
    {
        private readonly Lazy<AppDbContext> _db;
        private const double StandardHoursPerDay = 8.0;

        public ShiftReportService(Lazy<AppDbContext> db)
        {
            _db = db;
        }
        private AppDbContext db => _db.Value;
        public CompanyReportDto GetCompanyReport(int companyId, DateTime fromUtc, DateTime toUtc)
        {
            var company = db.CompanyModel.Find(companyId);
            if (company == null)
            {
                throw new CustomException("Company not found", ExceptionLevel.Warning);
            }

            var employees = db.UserModel.Where(u => u.CompanyId == companyId).ToList();

            var shifts = db.Shifts.Where(s => s.CompanyId == companyId &&
                                               s.StartUtc >= fromUtc &&
                                               s.EndUtc <= toUtc).ToList();

            var report = new CompanyReportDto
            {
                CompanyId = companyId,
                CompanyName = company.Name,
                PeriodStart = fromUtc,
                PeriodEnd = toUtc
            };

            foreach (var emp in employees)
            {
                var empShifts = shifts.Where(s => s.OwnerUserId == emp.Id || !s.OwnerUserId.HasValue).ToList();
                var totalHours = empShifts.Sum(s => (s.EndUtc - s.StartUtc).TotalHours);
                var offDays = empShifts.Count(s => s.IsOffDay);
                var overtime = totalHours > StandardHoursPerDay * empShifts.Count ? totalHours - (StandardHoursPerDay * empShifts.Count) : 0;

                report.EmployeeSummaries.Add(new ShiftSummaryDto
                {
                    UserId = emp.Id,
                    UserName = emp.Name,
                    TotalShifts = empShifts.Count,
                    TotalHours = Math.Round(totalHours, 2),
                    OvertimeHours = Math.Round(overtime, 2),
                    OffDays = offDays
                });
            }
            return report;
        }

        public EmployeeReportDto GetEmployeeReport(int userId, DateTime fromUtc, DateTime toUtc)
        {
            var user = db.UserModel.Find(userId);
            if (user == null)
            {
                throw new CustomException("User not found", ExceptionLevel.Warning);
            }

            var shifts = db.Shifts.Where(s => s.OwnerUserId == userId ||
                                               (s.CompanyId == user.CompanyId && !s.OwnerUserId.HasValue))
                                   .Where(s => s.StartUtc >= fromUtc && s.EndUtc <= toUtc)
                                   .ToList();

            var totalHours = shifts.Sum(s => (s.EndUtc - s.StartUtc).TotalHours);
            var offDays = shifts.Count(s => s.IsOffDay);
            var overtime = totalHours > StandardHoursPerDay * shifts.Count ? totalHours - (StandardHoursPerDay * shifts.Count) : 0;

            return new EmployeeReportDto
            {
                UserId = user.Id,
                UserName = user.Name,
                PeriodStart = fromUtc,
                PeriodEnd = toUtc,
                TotalHours = Math.Round(totalHours, 2),
                OvertimeHours = Math.Round(overtime, 2),
                OffDays = offDays
            };
        }
    }
}
