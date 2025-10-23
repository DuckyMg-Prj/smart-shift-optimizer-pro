using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.Enums;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Ui.Api.Resourcess;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

[Authorize]
[RoutePrefix("api/company")]
public class CompanyController : ApiController
{
    private readonly Lazy<ICompanyService> _companyService;

    public CompanyController(Lazy<ICompanyService> companyService)
    {
        _companyService = companyService;
    }
    private ICompanyService companyService => _companyService.Value;
    private int GetCurrentUserId()
    {
        var idClaim = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "id");
        return idClaim != null ? int.Parse(idClaim.Value) : 0;
    }

    [HttpPost, Route("create")]
    public IHttpActionResult Create(CompanyCreateDto dto)
    {
        var userId = GetCurrentUserId();
        var company = companyService.CreateCompany(userId, dto);
        return Ok(new { message = MessageResources.cCompany_GetCurrent_CompanyCreated, companyId = company.Id, invite = company.InviteCode });
    }

    [HttpPost, Route("join")]
    public IHttpActionResult Join(CompanyJoinDto dto)
    {
        var userId = GetCurrentUserId();
        var ok = companyService.JoinCompanyByInvite(userId, dto.InviteCode);
        if (!ok)
        {
            return BadRequest(MessageResources.cCompany_Join_InvalidInviteCode);
        }
        return Ok(new { message = MessageResources.cCompany_Join_RegisterSuccess });
    }

    [HttpGet, Route("mine")]
    public IHttpActionResult MyCompany()
    {
        var userId = GetCurrentUserId();
        var companies = companyService.GetCompaniesForUser(userId).ToList();
        return Ok(companies);
    }

    [HttpGet, Route("{id}")]
    public IHttpActionResult Get(int id)
    {
        var company = companyService.GetCompany(id);
        if (company == null) return NotFound();
        return Ok(company);
    }
    private bool IsEmployer()
    {
        var r = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        return r != null && r == ((byte)RoleTypeKind.Employer).ToString();
    }
}
