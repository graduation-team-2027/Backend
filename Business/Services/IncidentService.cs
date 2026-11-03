using AISmokingDetection.Business.Interfaces;

using AISmokingDetection.DTOs.Evidence;
using AISmokingDetection.DTOs.Incident;
using AISmokingDetection.DTOs.ProcessingJob;
using AISmokingDetection.Data;

using AISmokingDetection.Model;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Business.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly ApplicationDbContext _context;

        public IncidentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IncidentResponseDto> CreateIncidentAsync(CreateIncidentDto dto)
        {
            bool eventExists = await _context.Incidents
                .AnyAsync(i => i.EventId == dto.EventId);

            if (eventExists)
                throw new InvalidOperationException("This event already exists.");

            var incident = new Incident
            {
                EventId = dto.EventId,
                CameraId = dto.CameraId,
                ParticipantId = dto.ParticipantId,
                EventType = dto.EventType,
                ConfidenceScore = dto.ConfidenceScore,
                Status = IncidentStatus.Suspected,
                IdentityStatus = IdentityStatus.Unknown,
                ReviewStatus = ReviewStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Incidents.Add(incident);

            await _context.SaveChangesAsync();

            return MapToResponseDto(incident);
        }

        public async Task<List<IncidentResponseDto>> GetAllIncidentsAsync()
        {
            var incidents = await _context.Incidents
                .Include(i => i.Evidences)
                .Include(i => i.ProcessingJobs)
                .ToListAsync();

            return incidents.Select(MapToResponseDto).ToList();
        }

        public async Task<IncidentResponseDto?> GetIncidentByIdAsync(int id)
        {
            var incident = await _context.Incidents
                .Include(i => i.Evidences)
                .Include(i => i.ProcessingJobs)
                .FirstOrDefaultAsync(i => i.IncidentId == id);

            if (incident == null)
                return null;

            return MapToResponseDto(incident);
        }

        private static IncidentResponseDto MapToResponseDto(Incident incident)
        {
            return new IncidentResponseDto
            {
                IncidentId = incident.IncidentId,
                EventId = incident.EventId,
                CameraId = incident.CameraId,
                ParticipantId = incident.ParticipantId,
                EventType = incident.EventType,
                Status = incident.Status,
                IdentityStatus = incident.IdentityStatus,
                ReviewStatus = incident.ReviewStatus,
                ConfidenceScore = incident.ConfidenceScore,
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt,

                Evidences = incident.Evidences.Select(e => new EvidenceResponseDto
                {
                    EvidenceId = e.EvidenceId,
                    IncidentId = e.IncidentId,
                    EvidenceType = e.EvidenceType,
                    StorageReference = e.StorageReference,
                    AvailabilityStatus = e.AvailabilityStatus
                }).ToList(),

                ProcessingJobs = incident.ProcessingJobs.Select(p => new ProcessingJobResponseDto
                {
                    ProcessingJobId = p.ProcessingJobId,
                    IncidentId = p.IncidentId,
                    Status = p.Status,
                    ModelVersion = p.ModelVersion,
                    DataOrigin = p.DataOrigin,
                    StartedAtUtc = p.StartedAtUtc,
                    CompletedAtUtc = p.CompletedAtUtc,
                    ErrorCode = p.ErrorCode
                }).ToList()
            };
        }
    }
}