using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.DTOs.Evidence;
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

        [HttpPost]
        public async Task<IActionResult> CreateEvidence(CreateEvidenceDto dto)
        {
            var result = await _evidenceService.CreateEvidenceAsync(dto);

            return CreatedAtAction(
                nameof(GetEvidenceById),
                new { id = result.EvidenceId },
                result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvidenceById(Guid id)
        {
            var result = await _evidenceService.GetEvidenceByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("incident/{incidentId:int}")]
        public async Task<IActionResult> GetEvidenceByIncidentId(int incidentId)
        {
            var result = await _evidenceService
                .GetEvidenceByIncidentIdAsync(incidentId);

            return Ok(result);
        }
    }
}