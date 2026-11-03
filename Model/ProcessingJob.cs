namespace AISmokingDetection.Model
{
    public enum ProcessingJobStatus
    {
        Queued,
        Processing,
        Completed,
        Failed
    }


    public class ProcessingJob
    {
        public int ProcessingJobId { get; set; }

        public int? IncidentId { get; set; }

        public ProcessingJobStatus Status { get; set; }

        public string? ModelVersion { get; set; }

        public string DataOrigin { get; set; } = "fixture";

        public DateTimeOffset? StartedAtUtc { get; set; }

        public DateTimeOffset? CompletedAtUtc { get; set; }

        public string? ErrorCode { get; set; }

        // Navigation Property
        public Incident? Incident { get; set; }
    }
}

