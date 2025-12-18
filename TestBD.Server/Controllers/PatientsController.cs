using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestBD.Server.Controllers.Base;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;
using TestBD.Server.Services;

namespace TestBD.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientsController(PatientService service) : BaseController
    {
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("patients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var response = await service.GetPatientsAsync();
            return Ok(response);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("addPatient")]
        public async Task<IActionResult> AddAsync([FromBody] PatientDto patient)
        {
            await service.AddPatientAsync(patient);
            return Created();
        }
        [Authorize(Policy = "UserOnly")]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var responce = await service.GetById(id);
            return Ok(responce);
        }

        [Authorize(Policy = "UserOrAdmin")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] PatientDto request)
        {
            await service.UpdateAsync(request);
            return Ok();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{patient_id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid patient_id)
        {
            await service.DeleteAsync(patient_id);
            return NoContent();
        }
    }
}
