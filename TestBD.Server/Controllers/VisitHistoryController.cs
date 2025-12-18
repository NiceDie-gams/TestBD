using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestBD.Server.Controllers.Base;
using TestBD.Server.Dtos;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitHistoryController(VisitHistoryService service) : BaseController
    {
        [Authorize(Policy = "UserOnly")]
        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetByPatientId([FromRoute]Guid patientId)
        {
            var responce = await service.GetByPatientId(patientId);
            return Ok(responce);
        }

        [Authorize(Policy = "EmployeeOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddAsync(VisitHistoryRequestDto request)
        {
            await service.AddAsync(request);
            return Created();
        }
    }
}
