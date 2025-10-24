using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Users;
using SmartShift.Core.Model.DTOs.Users.Shift;
using SmartShift.Core.Model.Entities;
using System.Collections.Generic;

namespace SmartShift.Core.Service.Abstraction
{
    public interface IShiftAssignmentService
    {
        ShiftAssignmentDto AssignEmployee(int employerId, int shiftId, int employeeId);
        bool UnassignEmployee(int employerId, int shiftId, int employeeId);
        IEnumerable<ShiftAssignmentDto> GetAssignmentsByShift(int shiftId);
        IEnumerable<ShiftAssignmentDto> GetAssignmentsByEmployee(int employeeId);
    }
}