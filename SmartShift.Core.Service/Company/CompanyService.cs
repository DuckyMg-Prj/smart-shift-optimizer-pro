using SmartShift.Core.Data.Context;
using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.Entities;
using SmartShift.Core.Model.Source;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Core.Service.Resourcess;
using System;
using System.Collections.Generic;
using System.Linq;

public class CompanyService : ICompanyService
{
    private readonly Lazy<AppDbContext> _db;

    public CompanyService(Lazy<AppDbContext> db)
    {
        _db = db;
    }
    private AppDbContext db => _db.Value;
    public CompanyModel CreateCompany(int creatorUserId, CompanyCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new CustomException(ExceptionResources.sCompanyService_CreateCompany_MissingCompanyName, ExceptionLevel.Warning);
        }

        var invite = GenerateInviteCode();
        var company = new CompanyModel
        {
            Name = dto.Name,
            InviteCode = invite,
            CreatedBy = creatorUserId,
            CreatedAt = DateTime.UtcNow
        };

        db.CompanyModel.Add(company);
        db.SaveChanges();

        var creator = db.UserModel.Find(creatorUserId);
        if (creator != null)
        {
            creator.CompanyId = company.Id;
            db.SaveChanges();
        }

        return company;
    }

    public CompanyDto GetCompany(int companyId)
    {
        var c = db.CompanyModel.Find(companyId);
        if (c == null) return null;
        return new CompanyDto { Id = c.Id, Name = c.Name, InviteCode = c.InviteCode, CreatedBy = c.CreatedBy };
    }

    public IEnumerable<CompanyDto> GetCompaniesForUser(int userId)
    {
        var user = db.UserModel.Find(userId);
        if (user == null)
        {
            yield return (CompanyDto)Enumerable.Empty<CompanyDto>();
        }

        if (user.CompanyId.HasValue)
        {
            var c = db.CompanyModel.Find(user.CompanyId.Value);
            if (c != null) yield return new CompanyDto { Id = c.Id, Name = c.Name, InviteCode = c.InviteCode, CreatedBy = c.CreatedBy };
        }
    }

    public bool JoinCompanyByInvite(int userId, string inviteCode)
    {
        var company = db.CompanyModel.FirstOrDefault(x => x.InviteCode == inviteCode);
        if (company == null)
        {
            return false;
        }
        var user = db.UserModel.Find(userId);
        if (user == null)
        {
            return false;
        }

        user.CompanyId = company.Id;
        db.SaveChanges();
        return true;
    }

    public string GetInviteCode(int companyId)
    {
        var c = db.CompanyModel.Find(companyId);
        return c?.InviteCode;
    }

    private string GenerateInviteCode()
    {

        return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
    }
}
