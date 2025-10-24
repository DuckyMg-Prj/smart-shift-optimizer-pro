using SmartShift.Core.Model.DTOs.Users.Shift;
using SmartShift.Core.Service.Abstraction;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

[Authorize]
[RoutePrefix("api/requests")]
public class ShiftRequestsController : ApiController
{
    private readonly IShiftRequestService _service;
    public ShiftRequestsController(IShiftRequestService service) { _service = service; }

    [HttpPost, Route("create")]
    public IHttpActionResult Create(ShiftRequestCreateDto dto)
    {
        var userId = GetUserId();
        var res = _service.CreateRequest(userId, dto);
        return Ok(res);
    }

    [HttpPost, Route("{id}/approve")]
    public IHttpActionResult Approve(int id)
    {
        var employerId = GetUserId();
        var ok = _service.ApproveRequest(employerId, id);
        if (ok)
        {
            return Ok();
        }
        else
        {
            return BadRequest("Not found");
        }
    }

    [HttpPost, Route("{id}/reject")]
    public IHttpActionResult Reject(int id)
    {
        var employerId = GetUserId();
        var ok = _service.RejectRequest(employerId, id);
        if (ok)
        {
            return Ok();
        }
        else
        {
            return BadRequest("Not found");
        }
    }

    [HttpGet, Route("mine")]
    public IHttpActionResult Mine()
    {
        var userId = GetUserId();
        return Ok(_service.GetRequestsByEmployee(userId));
    }

    [HttpGet, Route("company/{companyId}")]
    public IHttpActionResult Company(int companyId)
    {
        return Ok(_service.GetRequestsForCompany(companyId));
    }

    private int GetUserId()
    {
        var idClaim = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "id");
        return idClaim != null ? int.Parse(idClaim.Value) : 0;
    }
}
