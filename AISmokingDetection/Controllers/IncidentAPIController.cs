using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.DTOs.Incident;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISmokingDetection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIncident(CreateIncidentDto dto)
        {
            var result = await _incidentService.CreateIncidentAsync(dto);

            return CreatedAtAction(
                nameof(GetIncidentById),
                new { id = result.IncidentId },
                result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllIncidents()
        {
            var result = await _incidentService.GetAllIncidentsAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetIncidentById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid incident ID.");

            var result = await _incidentService.GetIncidentByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
