using Microsoft.AspNetCore.Mvc;
using TestBD.Server.Controllers.Base;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController(UslugiService service) : BaseController
    {
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices()
        {
            var responce = await service.GetAllServices();
            return Ok(responce);
        }
    }
}
