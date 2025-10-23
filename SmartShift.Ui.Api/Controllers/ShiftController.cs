using SmartShift.Core.Model.DTOs;
using SmartShift.Core.Model.DTOs.Users.Shift;
using SmartShift.Core.Model.Source;
using SmartShift.Core.Service.Abstraction;
using SmartShift.Ui.Api.Resourcess;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace SmartShift.Ui.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/shifts")]
    public class ShiftController : ApiController
    {
        private readonly Lazy<IShiftService> _shiftService;
        public ShiftController(Lazy<IShiftService> shiftService) {
            _shiftService = shiftService; 
        }
       private IShiftService ShiftService => _shiftService.Value;

        private int GetUserId()
        {
            var idClaim = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "id");
            return idClaim != null ? int.Parse(idClaim.Value) : 0;
        }

        [HttpPost, Route("create")]
        public IHttpActionResult Create(ShiftCreateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var shift = ShiftService.CreateShift(userId, dto);
                return Ok(new { message = MessageResources.cShift_Create_RegisterSuccess, shiftId = shift.Id });
            }
            catch (CustomException ex)
            {
                return Content(ex.Level == ExceptionLevel.Warning ? System.Net.HttpStatusCode.BadRequest : System.Net.HttpStatusCode.InternalServerError,
                    new { level = ex.Level.ToString(), message = ex.Message });
            }
        }

        [HttpPut, Route("{id}")]
        public IHttpActionResult Update(int id, ShiftUpdateDto dto)
        {
            try
            {
                var userId = GetUserId();
                var shift = ShiftService.UpdateShift(userId, id, dto);
                return Ok(new { message = MessageResources.cShift_Update_RegisterSuccess, shiftId = shift.Id });
            }
            catch (CustomException ex)
            {
                return Content(ex.Level == ExceptionLevel.Warning ? System.Net.HttpStatusCode.BadRequest : System.Net.HttpStatusCode.InternalServerError,
                    new { level = ex.Level.ToString(), message = ex.Message });
            }
        }

        [HttpDelete, Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var userId = GetUserId();
            var ok = ShiftService.DeleteShift(userId, id);
            if (!ok)
            {
                return BadRequest(MessageResources.cShift_Delete_CanNotDelete);

            }
            return Ok(new { message = MessageResources.cShift_Delete_RegisterSuccess });
        }

        [HttpGet, Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var userId = GetUserId();
            var dto = ShiftService.GetShift(userId, id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpGet, Route("list")]
        public IHttpActionResult List([FromUri] ShiftQueryDto query)
        {
            var userId = GetUserId();
            var result = ShiftService.QueryShifts(userId, query ?? new ShiftQueryDto());
            return Ok(result);
        }
    }
}
