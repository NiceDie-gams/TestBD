using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestBD.Server.Controllers.Base;
using TestBD.Server.Dtos;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController(EmployeeService service) : BaseController
    {

        [HttpGet("employee")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var responce = await service.GetAllDoctors();
            return Ok(responce);
        }

        [HttpGet("employee/post")]
        public async Task<IActionResult> GetDoctorByPost([FromQuery]string post)
        {
            var responce = await service.GetEmployeeByPost(post);
            return Ok(responce);
        }

        [Authorize(Policy ="AdminOnly")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody]EmployeeDto dto)
        {
            await service.UpdateAsync(dto);
            return Ok(dto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await service.DeleteAsync(id);
            return NoContent();
        }
    }
}
