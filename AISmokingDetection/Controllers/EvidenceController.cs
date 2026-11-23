using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.DTOs.Evidence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISmokingDetection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvidenceController : ControllerBase
    {
        private readonly IEvidenceService _evidenceService;

        public EvidenceController(IEvidenceService evidenceService)
        {
            _evidenceService = evidenceService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateEvidence(CreateEvidenceDto dto)
        {
            try
            {
                var result = await _evidenceService.CreateEvidenceAsync(dto);

                return CreatedAtAction(
                    nameof(GetEvidenceById),
                    new { id = result.EvidenceId },
                    result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("incident/{incidentId:int}")]
        public async Task<IActionResult> GetEvidenceByIncidentId(int incidentId)
        {
            if (incidentId <= 0)
                return BadRequest("Invalid incident ID.");

            var result = await _evidenceService.GetEvidenceByIncidentIdAsync(incidentId);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEvidenceById(Guid id)
        {
            var result = await _evidenceService.GetEvidenceByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
