using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using TestBD.Server.Controllers.Base;
using TestBD.Server.Dtos;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AppointmentController(AppointmentService service) : BaseController
    {
        [Authorize(Policy ="UserOnly")]
        [HttpGet("ByPatientId/{patientId}")]
        public async Task<IActionResult> GetAllAppointmentByID([FromRoute]Guid patientId)
        {
            var responce = await service.GetAllByPatientId(patientId);
            return Ok(responce);
        }

        [Authorize(Policy = "UserOnly")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody]AppointmentRequestDto request)
        {
            await service.AddAsync(request);
            return Ok(request);
        }

        [Authorize(Policy = "UserOnly")]
        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromQuery]Guid id)
        {
            await service.UpdateStatus(id);
            return Ok();
        }

        [Authorize(Policy = "EmployeeOrAdmin")]
        [HttpGet("completed/{employeeId}")]
        public async Task<IActionResult> GetAllCompletedAsync([FromRoute] Guid employeeId)
        {
            var responce = await service.GetAllCompleteAppointmentsForEmployee(employeeId);
            return Ok(responce);
        }

        [Authorize(Policy ="AdminOnly")]
        [HttpGet("allToday")]
        public async Task<IActionResult> GetAllToday()
        {
            var responce = await service.GetAllTodayAppointment();
            return Ok(responce);
        }
    }
}
