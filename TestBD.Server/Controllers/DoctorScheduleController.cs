using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestBD.Server.Dtos;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoctorScheduleController(DoctorScheduleService service): ControllerBase
    {
        //[Authorize(Roles ="user")]
        [HttpGet]
        public async Task<IActionResult> GetByEmployeeId(Guid employeeId, DateOnly date)
        {
            var responce = await service.GetByEmployeeIdAndDate(employeeId, date);
            return Ok(responce);
        }

        [Authorize(Policy = "EmployeeOrAdmin")]
        [HttpGet("GetWithAppointment")]
        public async Task<IActionResult> GetWithAppointmentAsync([FromQuery]Guid id, [FromQuery]DateOnly date)
        {
            var responce = await service.GetScheduleWithAppointmentsAsyn(id, date);
            return Ok(responce);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("createSchedule")]
        public async Task<IActionResult> CreateSchedule([FromBody]CreateScheduleDto request)
        {
            await service.CreateDoctorSchedule(request);
            return Created();
        }
    }
}
