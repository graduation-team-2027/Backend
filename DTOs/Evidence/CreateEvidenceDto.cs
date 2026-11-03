namespace AISmokingDetection.DTOs.Evidence
{
    public class CreateEvidenceDto
    {
        public int IncidentId { get; set; }

        public string EvidenceType { get; set; } = string.Empty;

        public string? StorageReference { get; set; }

        public string AvailabilityStatus { get; set; } = string.Empty;
    }
}