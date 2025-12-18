using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestBD.Server.Controllers.Base;
using TestBD.Server.Dtos;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController(PaymentService service) : BaseController
    {
        [Authorize(Policy ="AdminOnly")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var responce = await service.GetAllAsync();
            return Ok(responce);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromBody]UpdatePaymentStatusDto request)
        {
            await service.UpdateStatusAsync(request);
            return Ok();
        }
    }
}
