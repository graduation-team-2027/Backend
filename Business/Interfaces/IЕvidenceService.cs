using AISmokingDetection.DTOs.Evidence;

namespace AISmokingDetection.Business.Interfaces
{
    
        public interface IEvidenceService
        {
            Task<EvidenceResponseDto> CreateEvidenceAsync(CreateEvidenceDto dto);

            Task<List<EvidenceResponseDto>> GetEvidenceByIncidentIdAsync(int incidentId);

            Task<EvidenceResponseDto?> GetEvidenceByIdAsync(Guid id);
        }
    }

