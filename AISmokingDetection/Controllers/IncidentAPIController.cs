using AISmokingDetection.Data;
using AISmokingDetection.Model;
using AISmokingDetection.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IncidentAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/IncidentAPI
        [HttpGet("GetIncidents")]
     
        public async Task<IActionResult> GetIncidents()
        {
            var incidents = await _context.Incidents
                .Select(i => new
                {
                    i.IncidentId,
                    i.EventId,
                    i.EventType,
                    i.Status,
                    i.IdentityStatus,
                   
                    i.ConfidenceScore,
                    i.DataOrigin,
                    i.ModelVersion,
                    i.Source,
                    i.CreatedAt,
                    i.UpdatedAt,

                    Evidence = i.Evidence.Select(e => new
                    {
                        e.EvidenceId,
                        e.IncidentId,
                        e.EvidenceType,
                        e.FileUrl,
                        e.AvailabilityStatus,
                        e.Metadata,
                        e.CreatedAt
                    }).ToList()
                })
                .ToListAsync();

            return Ok(incidents);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncident(string id)
        {
            var incident = await _context.Incidents
                .Where(i => i.IncidentId == id)
                .Select(i => new
                {
                    i.IncidentId,
                    i.EventId,
                    i.EventType,
                    i.Status,
                    i.IdentityStatus,
                   
                    i.ConfidenceScore,
                    i.DataOrigin,
                    i.ModelVersion,
                    i.Source,
                    i.CreatedAt,
                    i.UpdatedAt,

                    Evidence = i.Evidence.Select(e => new
                    {
                        e.EvidenceId,
                        e.IncidentId,
                        e.EvidenceType,
                        e.FileUrl,
                        e.AvailabilityStatus,
                        e.Metadata,
                        e.CreatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (incident == null)
                return NotFound();

            return Ok(incident);
        }

        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetIncidentReviews(string id)
        {
            var incidentExists = await _context.Incidents
                .AnyAsync(i => i.IncidentId == id);

            if (!incidentExists)
            {
                return NotFound(new
                {
                    message = $"Incident with ID {id} was not found."
                });
            }

            var reviews = await _context.ReviewHistories
                .Where(r => r.IncidentId == id)
                .OrderByDescending(r => r.ReviewedAt)
                .Select(r => new
                {
                    r.ReviewHistoryId,
                    r.IncidentId,
                 
                    r.ReviewReason,
                    r.ReviewerId,
                    r.ReviewedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpPost]
        public async Task<ActionResult<Incident>> CreateIncident(
            CreateIncidentDto dto)
        {
            if (dto.EventId == Guid.Empty ||
                            string.IsNullOrWhiteSpace(dto.EventType))
            {
                return BadRequest("EventId and EventType are required.");
            }

            var incident = new Incident
            {
                IncidentId = Guid.NewGuid().ToString(),
                EventId = dto.EventId,
                EventType = dto.EventType,
                ConfidenceScore = dto.ConfidenceScore,
                ModelVersion = dto.ModelVersion,
                Source = dto.Source,
                Status = "suspected",
                IdentityStatus = "unknown",
                DataOrigin = "fixture",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetIncident),
                new { id = incident.IncidentId },
                incident);
        }

            [HttpPost("{id}/evidence")]
        public async Task<IActionResult> AddEvidence(
            string id,
            CreateEvidenceDto dto)
        {
            var incident = await _context.Incidents
                .FirstOrDefaultAsync(i => i.IncidentId == id);

            if (incident == null)
            {
                return NotFound(new
                {
                    message = $"Incident with ID {id} was not found."
                });
            }

            if (dto.EvidenceType != "image" &&
                dto.EvidenceType != "video")
            {
                return BadRequest(new
                {
                    message = "EvidenceType must be image or video."
                });
            }

            var evidence = new Evidence
            {
                IncidentId = id,
                EvidenceType = dto.EvidenceType,
                FileUrl = dto.FileUrl,
                AvailabilityStatus = dto.AvailabilityStatus,
                Metadata = dto.Metadata,
                CreatedAt = DateTime.UtcNow
            };

            _context.Evidences.Add(evidence);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                evidence.EvidenceId,
                evidence.IncidentId,
                evidence.EvidenceType,
                evidence.FileUrl,
                evidence.AvailabilityStatus,
                evidence.Metadata,
                evidence.CreatedAt
            });
        }


        [HttpPut("{id}/review")]
        public async Task<IActionResult> ReviewIncident(
            string id,
            ReviewIncidentDto dto)
        {
            var incident = await _context.Incidents
                .FirstOrDefaultAsync(i => i.IncidentId == id);

            if (incident == null)
            {
                return NotFound(new
                {
                    message = $"Incident with ID {id} was not found."
                });
            }

            var allowedStatuses = new[]
            {
        "confirmed",
        "rejected",
        "unresolved",
        "resolved"
    };

            if (!allowedStatuses.Contains(dto.NewStatus))
            {
                return BadRequest(new
                {
                    message = "Invalid status."
                });
            }

            var previousStatus = incident.Status;

            var review = new ReviewHistory
            {
                IncidentId = incident.IncidentId,
               
                ReviewReason = dto.ReviewReason,
                ReviewerId = dto.ReviewerId,
                ReviewedAt = DateTime.UtcNow
            };

            incident.Status = dto.NewStatus;
            incident.UpdatedAt = DateTime.UtcNow;

            _context.ReviewHistories.Add(review);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Incident reviewed successfully.",
                incidentId = incident.IncidentId,
                previousStatus,
                newStatus = incident.Status
            });
        }


    }
}
