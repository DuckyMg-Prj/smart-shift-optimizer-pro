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


    public class ShiftRequestService : IShiftRequestService
    {
        private readonly Lazy<AppDbContext> _db;
        public ShiftRequestService(Lazy<AppDbContext> db)
        {
            _db = db;
        }
        private AppDbContext db => _db.Value;
        public ShiftRequestDto CreateRequest(int userId, ShiftRequestCreateDto dto)
        {
            var shift = db.Shifts.Find(dto.ShiftId);
            if (shift == null)
            {
                throw new CustomException("Shift not found", ExceptionLevel.Warning);
            }
            Enum.TryParse<ShiftRequestType>(dto.Type, true, out var typeResult);
            var request = new ShiftRequest
            {
                ShiftId = dto.ShiftId,
                RequestedByUserId = userId,
                TargetUserId = dto.TargetUserId,
                Type = typeResult,
                Status = ShiftRequestStatus.Pending
            };

            db.ShiftRequests.Add(request);
            db.SaveChanges();

            return ToDto(request);
        }

        public bool ApproveRequest(int employerId, int requestId)
        {
            var req = db.ShiftRequests.Find(requestId);
            if (req == null) return false;
            req.Status = ShiftRequestStatus.Approved;

            // swap assignment if needed
            if (req.Type == ShiftRequestType.Swap && req.TargetUserId.HasValue)
            {
                var assign1 = db.ShiftAssignments.FirstOrDefault(a => a.ShiftId == req.ShiftId && a.EmployeeId == req.RequestedByUserId);
                var assign2 = db.ShiftAssignments.FirstOrDefault(a => a.ShiftId == req.ShiftId && a.EmployeeId == req.TargetUserId);

                if (assign1 != null)
                {
                    db.ShiftAssignments.Remove(assign1);
                    db.ShiftAssignments.Add(new ShiftAssignment
                    {
                        ShiftId = req.ShiftId,
                        EmployeeId = req.TargetUserId.Value,
                        IsConfirmed = true
                    });
                }
            }

            db.SaveChanges();
            return true;
        }

        public bool RejectRequest(int employerId, int requestId)
        {
            var req = db.ShiftRequests.Find(requestId);
            if (req == null) return false;
            req.Status = ShiftRequestStatus.Rejected;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<ShiftRequestDto> GetRequestsForCompany(int companyId)
        {
            var data = from r in db.ShiftRequests
                       join s in db.Shifts on r.ShiftId equals s.Id
                       join u in db.UserModel on r.RequestedByUserId equals u.Id
                       where s.CompanyId == companyId
                       select ToDto(r, u.Name);
            return data.ToList();
        }

        public IEnumerable<ShiftRequestDto> GetRequestsByEmployee(int userId)
        {
            var data = from r in db.ShiftRequests
                       join u in db.UserModel on r.RequestedByUserId equals u.Id
                       where r.RequestedByUserId == userId
                       select ToDto(r, u.Name);
            return data.ToList();
        }

        private ShiftRequestDto ToDto(ShiftRequest r, string userName = null)
        {
            return new ShiftRequestDto
            {
                Id = r.Id,
                ShiftId = r.ShiftId,
                RequestedByUserId = r.RequestedByUserId,
                RequestedByName = userName ?? db.UserModel.Find(r.RequestedByUserId)?.Name,
                TargetUserId = r.TargetUserId,
                TargetUserName = r.TargetUserId.HasValue ? db.UserModel.Find(r.TargetUserId)?.Name : null,
                Type = r.Type.ToString(),
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt
            };
        }
    }
}
