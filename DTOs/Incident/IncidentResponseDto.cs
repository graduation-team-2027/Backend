using AISmokingDetection.DTOs.Evidence;
using AISmokingDetection.DTOs.ProcessingJob;
using AISmokingDetection.Model;

namespace AISmokingDetection.DTOs.Incident
{
    public class IncidentResponseDto
    {
        public int IncidentId { get; set; }

        public Guid EventId { get; set; }

        public int CameraId { get; set; }

        public int ParticipantId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public IncidentStatus Status { get; set; }

        public IdentityStatus IdentityStatus { get; set; }

        public ReviewStatus ReviewStatus { get; set; }

        public decimal? ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<EvidenceResponseDto> Evidences { get; set; } = new();

        public List<ProcessingJobResponseDto> ProcessingJobs { get; set; } = new();
    }
}