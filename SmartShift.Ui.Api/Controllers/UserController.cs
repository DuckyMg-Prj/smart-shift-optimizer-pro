using SmartShift.Core.Data.Context;
using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Service.Abstraction;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

[Authorize]
[RoutePrefix("api/user")]
public class UserController : ApiController
{
    private readonly AppDbContext _db;

    public UserController()
    {
        _db = new AppDbContext(); // or inject IUserService in DI; keep simple for now
    }

    private int GetCurrentUserId()
    {
        var idClaim = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "id");
        return idClaim != null ? int.Parse(idClaim.Value) : 0;
    }

    [HttpGet, Route("me")]
    public IHttpActionResult Me()
    {
        var id = GetCurrentUserId();
        var u = _db.UserModel.Find(id);
        if (u == null)
        {
            return NotFound();
        }
        var dto = new UserProfileDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            CompanyId = u.CompanyId
        };
        return Ok(dto);
    }
}
