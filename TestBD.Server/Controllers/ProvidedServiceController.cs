using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestBD.Server.Dtos;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProvidedServiceController(ProvidedUslugaService service) : ControllerBase
    {
        [Authorize(Roles = "employee")]
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var responce = await service.GetByAppointmentId(id);
            return Ok(responce);
        }

        [Authorize(Roles = "employee")]
        [HttpPost]
        public async Task<IActionResult> AddAsync(ProvidedServiceDto request)
        {
            await service.AddServiceAsync(request);
            return Created();
        }

        [Authorize(Roles = "employee")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ProvidedServiceDto request)
        {
            await service.UpdateAsync(request);
            return Ok();
        }
    }
}
