using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.DTOs.Evidence;
using AISmokingDetection.Data;
using AISmokingDetection.Model;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.API.Business.Services
{
    public class EvidenceService : IEvidenceService
    {
        private readonly ApplicationDbContext _context;

        public EvidenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EvidenceResponseDto> CreateEvidenceAsync(
            CreateEvidenceDto dto)
        {
            bool incidentExists = await _context.Incidents
                .AnyAsync(i => i.IncidentId == dto.IncidentId);

            if (!incidentExists)
                throw new KeyNotFoundException("Incident not found.");

            var evidence = new Evidence
            {
                IncidentId = dto.IncidentId,
                EvidenceType = dto.EvidenceType,
                StorageReference = dto.StorageReference,
                AvailabilityStatus = dto.AvailabilityStatus,
                CreatedAt = DateTime.UtcNow
            };

            _context.Evidences.Add(evidence);

            await _context.SaveChangesAsync();

            return MapToResponseDto(evidence);
        }

        public async Task<List<EvidenceResponseDto>> GetEvidenceByIncidentIdAsync(
            int incidentId)
        {
            var evidences = await _context.Evidences
                .Where(e => e.IncidentId == incidentId)
                .ToListAsync();

            return evidences.Select(MapToResponseDto).ToList();
        }

        public async Task<EvidenceResponseDto?> GetEvidenceByIdAsync(Guid id)
        {
            var evidence = await _context.Evidences
                .FirstOrDefaultAsync(e => e.EvidenceId == id);

            if (evidence == null)
                return null;

            return MapToResponseDto(evidence);
        }

        private static EvidenceResponseDto MapToResponseDto(Evidence evidence)
        {
            return new EvidenceResponseDto
            {
                EvidenceId = evidence.EvidenceId,
                IncidentId = evidence.IncidentId,
                EvidenceType = evidence.EvidenceType,
                StorageReference = evidence.StorageReference,
                AvailabilityStatus = evidence.AvailabilityStatus,
                CreatedAt = evidence.CreatedAt
            };
        }
    }
}