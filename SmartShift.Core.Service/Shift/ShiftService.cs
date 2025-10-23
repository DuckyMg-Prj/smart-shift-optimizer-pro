using SmartShift.Core.Data.Context;
using SmartShift.Core.Model.DTOs;
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
    public class ShiftService : IShiftService
    {
        private readonly Lazy<AppDbContext> _db;

        public ShiftService(Lazy<AppDbContext> db)
        {
            _db = db;
        }
        private AppDbContext db => _db.Value;

        public Shift CreateShift(int userId, ShiftCreateDto dto)
        {

            var start = dto.StartLocal.ToUniversalTime();
            var end = dto.EndLocal.ToUniversalTime();
            if (end <= start)
            {
                throw new CustomException(ExceptionResources.sShift_CreateShift_EndDateLowerThanStart, ExceptionLevel.Warning);
            }


            if (dto.CompanyId.HasValue)
            {
                var user = db.UserModel.Find(userId);
                if (user == null)
                {
                    throw new CustomException(ExceptionResources.sShift_CreateShift_InvalidUserRole, ExceptionLevel.Error);
                }

                if (user.CompanyId != dto.CompanyId)
                {
                    throw new CustomException(ExceptionResources.sShift_CreateShift_NotMemberCompany , ExceptionLevel.Error);
                }
            }
            if (HasOverlap(dto.CompanyId, start, end))
            {
                throw new CustomException(ExceptionResources.sShift_CreateShift_ShiftOverLap, ExceptionLevel.Warning);
            }

            var shift = new Shift
            {
                CompanyId = dto.CompanyId,
                OwnerUserId = null,
                StartUtc = start,
                EndUtc = end,
                RequiredStaff = dto.RequiredStaff,
                IsOffDay = dto.IsOffDay,
                ClientId = dto.ClientId,
                CreatedAt = DateTime.UtcNow
            };

            db.Shifts.Add(shift);
            db.SaveChanges();
            return shift;
        }

        public Shift UpdateShift(int userId, int shiftId, ShiftUpdateDto dto)
        {
            var shift = db.Shifts.Find(shiftId);
            if (shift == null)
            {
                throw new CustomException(ExceptionResources.sShift_UpdateShift_ShiftNotFound, ExceptionLevel.Warning);
            }
            if (!CanEditShift(userId, shift))
            {
                throw new CustomException(ExceptionResources.sShift_UpdateShift_NoAccess, ExceptionLevel.Error);
            }

            var start = dto.StartLocal.ToUniversalTime();
            var end = dto.EndLocal.ToUniversalTime();
            if (end <= start)
            {
                throw new CustomException(ExceptionResources.sShift_CreateShift_EndDateLowerThanStart, ExceptionLevel.Warning);
            }

            if (HasOverlap(shift.CompanyId, start, end, excludeShiftId: shiftId))
            {
                throw new CustomException(ExceptionResources.sShift_CreateShift_ShiftOverLap, ExceptionLevel.Warning);
            }

            shift.StartUtc = start;
            shift.EndUtc = end;
            shift.RequiredStaff = dto.RequiredStaff;
            shift.IsOffDay = dto.IsOffDay;
            db.SaveChanges();
            return shift;
        }

        public bool DeleteShift(int userId, int shiftId)
        {
            var shift = db.Shifts.Find(shiftId);
            if (shift == null) return false;
            if (!CanEditShift(userId, shift))
            {
                throw new CustomException(ExceptionResources.sShift_UpdateShift_ShiftNotFound, ExceptionLevel.Error);
            }

            db.Shifts.Remove(shift);
            db.SaveChanges();
            return true;
        }

        public ShiftDto GetShift(int userId, int shiftId)
        {
            var s = db.Shifts.Find(shiftId);
            if (s == null)
            {
                return null;
            }
            return ToDto(s);
        }

        public IEnumerable<ShiftDto> QueryShifts(int userId, ShiftQueryDto query)
        {
            var q = db.Shifts.AsQueryable();
            if (query.CompanyId.HasValue)
            {
                q = q.Where(x => x.CompanyId == query.CompanyId.Value);
            }
            if (query.FromUtc.HasValue)
            {
                q = q.Where(x => x.EndUtc >= query.FromUtc.Value);
            }
            if (query.ToUtc.HasValue)
            {
                q = q.Where(x => x.StartUtc <= query.ToUtc.Value);
            }
            return q.OrderBy(x => x.StartUtc).ToList().Select(ToDto);
        }

        public bool CanEditShift(int userId, Shift shift)
        {

            var user = db.UserModel.Find(userId);
            if (user == null)
            {
                return false;
            }
            if (user.Role == RoleTypeKind.SelfEmployee && shift.OwnerUserId == userId)
            {
                return true;
            }

            if (user.Role == RoleTypeKind.Employer && user.CompanyId.HasValue && shift.CompanyId == user.CompanyId)
            {
                return true;
            }
            return false;
        }

        private bool HasOverlap(int? companyId, DateTime start, DateTime end, int excludeShiftId = 0)
        {
            var q = db.Shifts.Where(s => s.CompanyId == companyId);
            if (excludeShiftId > 0)
            {
                q = q.Where(s => s.Id != excludeShiftId);
            }
            return q.Any(s => s.StartUtc < end && s.EndUtc > start);
        }

        private ShiftDto ToDto(Shift s) => new ShiftDto
        {
            Id = s.Id,
            CompanyId = s.CompanyId,
            OwnerUserId = s.OwnerUserId,
            StartUtc = s.StartUtc,
            EndUtc = s.EndUtc,
            RequiredStaff = s.RequiredStaff,
            IsOffDay = s.IsOffDay
        };
    }
}
