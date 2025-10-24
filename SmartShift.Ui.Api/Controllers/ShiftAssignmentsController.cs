using SmartShift.Core.Service.Abstraction;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

[Authorize]
[RoutePrefix("api/assignments")]
public class ShiftAssignmentsController : ApiController
{
    private readonly IShiftAssignmentService _service;
    public ShiftAssignmentsController(IShiftAssignmentService service) { _service = service; }

    [HttpPost, Route("assign")]
    public IHttpActionResult Assign(int shiftId, int employeeId)
    {
        var employerId = GetUserId();
        var res = _service.AssignEmployee(employerId, shiftId, employeeId);
        return Ok(res);
    }

    [HttpDelete, Route("unassign")]
    public IHttpActionResult Unassign(int shiftId, int employeeId)
    {
        var employerId = GetUserId();
        var ok = _service.UnassignEmployee(employerId, shiftId, employeeId);
        if (ok)
        {
            return Ok();
        }
        else
        {
            return BadRequest("Not found");
        }
    }

    [HttpGet, Route("byShift/{shiftId}")]
    public IHttpActionResult ByShift(int shiftId) => Ok(_service.GetAssignmentsByShift(shiftId));

    [HttpGet, Route("byEmployee/{employeeId}")]
    public IHttpActionResult ByEmployee(int employeeId) => Ok(_service.GetAssignmentsByEmployee(employeeId));

    private int GetUserId()
    {
        var idClaim = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "id");
        return idClaim != null ? int.Parse(idClaim.Value) : 0;
    }
}
