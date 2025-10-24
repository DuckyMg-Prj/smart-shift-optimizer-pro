using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Users;
using SmartShift.Core.Model.DTOs.Users.Shift;
using SmartShift.Core.Model.Entities;
using System.Collections.Generic;

namespace SmartShift.Core.Service.Abstraction
{
    public interface IShiftRequestService
    {
        ShiftRequestDto CreateRequest(int userId, ShiftRequestCreateDto dto);
        bool ApproveRequest(int employerId, int requestId);
        bool RejectRequest(int employerId, int requestId);
        IEnumerable<ShiftRequestDto> GetRequestsForCompany(int companyId);
        IEnumerable<ShiftRequestDto> GetRequestsByEmployee(int userId);
    }
}