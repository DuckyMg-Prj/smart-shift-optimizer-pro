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

    public class ShiftAssignmentService : IShiftAssignmentService
    {
        private readonly Lazy<AppDbContext> _db;
        public ShiftAssignmentService(Lazy<AppDbContext> db) {
            _db = db;
        }
        private AppDbContext db => _db.Value;
        public ShiftAssignmentDto AssignEmployee(int employerId, int shiftId, int employeeId)
        {
            var shift = db.Shifts.Find(shiftId);
            if (shift == null)
            {
                throw new CustomException("Shift not found", ExceptionLevel.Warning);
            }

            var emp = db.UserModel.Find(employeeId);
            if (emp == null)
            {
                throw new CustomException("Employee not found", ExceptionLevel.Warning);
            }
            if (shift.CompanyId != emp.CompanyId)
            {
                throw new CustomException("Employee not in company", ExceptionLevel.Error);

            }

            bool alreadyAssigned = db.ShiftAssignments.Any(a => a.ShiftId == shiftId && a.EmployeeId == employeeId);
            if (alreadyAssigned)
            {
                throw new CustomException("Already assigned", ExceptionLevel.Warning);
            }

            var assign = new ShiftAssignment { ShiftId = shiftId, EmployeeId = employeeId, IsConfirmed = true };
            db.ShiftAssignments.Add(assign);
            db.SaveChanges();

            return new ShiftAssignmentDto
            {
                Id = assign.Id,
                ShiftId = shiftId,
                EmployeeId = employeeId,
                EmployeeName = emp.Name,
                IsConfirmed = true
            };
        }

        public bool UnassignEmployee(int employerId, int shiftId, int employeeId)
        {
            var item = db.ShiftAssignments.FirstOrDefault(a => a.ShiftId == shiftId && a.EmployeeId == employeeId);
            if (item == null)
            {
                return false;
            }
            db.ShiftAssignments.Remove(item);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<ShiftAssignmentDto> GetAssignmentsByShift(int shiftId)
        {
            var data = from a in db.ShiftAssignments
                       join u in db.UserModel on a.EmployeeId equals u.Id
                       where a.ShiftId == shiftId
                       select new ShiftAssignmentDto
                       {
                           Id = a.Id,
                           ShiftId = a.ShiftId,
                           EmployeeId = a.EmployeeId,
                           EmployeeName = u.Name,
                           IsConfirmed = a.IsConfirmed
                       };
            return data.ToList();
        }

        public IEnumerable<ShiftAssignmentDto> GetAssignmentsByEmployee(int employeeId)
        {
            var data = from a in db.ShiftAssignments
                       join s in db.Shifts on a.ShiftId equals s.Id
                       where a.EmployeeId == employeeId
                       select new ShiftAssignmentDto
                       {
                           Id = a.Id,
                           ShiftId = a.ShiftId,
                           EmployeeId = employeeId,
                           EmployeeName = db.UserModel.Find(employeeId).Name,
                           IsConfirmed = a.IsConfirmed
                       };
            return data.ToList();
        }
    }
}
