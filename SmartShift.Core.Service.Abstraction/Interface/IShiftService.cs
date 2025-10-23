using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Users.Shift;
using SmartShift.Core.Model.Entities;
using System.Collections.Generic;

namespace SmartShift.Core.Service.Abstraction
{
    public interface IShiftService
    {
        Shift CreateShift(int userId, ShiftCreateDto dto);
        Shift UpdateShift(int userId, int shiftId, ShiftUpdateDto dto);
        bool DeleteShift(int userId, int shiftId);
        ShiftDto GetShift(int userId, int shiftId);
        IEnumerable<ShiftDto> QueryShifts(int userId, ShiftQueryDto query);
        bool CanEditShift(int userId, Shift shift); // for role checks
    }
}