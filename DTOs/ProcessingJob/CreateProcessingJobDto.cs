namespace AISmokingDetection.DTOs.ProcessingJob
{
    public class CreateProcessingJobDto
    {
        public int? IncidentId { get; set; }

        public string? ModelVersion { get; set; }

        public string DataOrigin { get; set; } = "fixture";
    }
}