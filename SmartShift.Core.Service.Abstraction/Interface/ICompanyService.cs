using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Company;
using SmartShift.Core.Model.Entities;
using System.Collections.Generic;
namespace SmartShift.Core.Service.Abstraction
{
    public interface ICompanyService
    {
        CompanyModel CreateCompany(int creatorUserId, CompanyCreateDto dto);
        CompanyDto GetCompany(int companyId);
        IEnumerable<CompanyDto> GetCompaniesForUser(int userId);
        bool JoinCompanyByInvite(int userId, string inviteCode);
        string GetInviteCode(int companyId);
    }
}
