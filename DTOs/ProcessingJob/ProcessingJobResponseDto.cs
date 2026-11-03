using AISmokingDetection.Model;

namespace AISmokingDetection.DTOs.ProcessingJob
{
    public class ProcessingJobResponseDto
    {
        public int ProcessingJobId { get; set; }

        public int? IncidentId { get; set; }

        public ProcessingJobStatus Status { get; set; }

        public string? ModelVersion { get; set; }

        public string DataOrigin { get; set; } = string.Empty;

        public DateTimeOffset? StartedAtUtc { get; set; }

        public DateTimeOffset? CompletedAtUtc { get; set; }

        public string? ErrorCode { get; set; }
    }
}