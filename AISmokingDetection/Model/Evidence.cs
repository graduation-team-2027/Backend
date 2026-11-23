
namespace AISmokingDetection.Model
{
    public class Evidence
    {
        public Guid EvidenceId { get; set; }

        public int IncidentId { get; set; }

        public string EvidenceType { get; set; } = string.Empty;

        public string StorageReference { get; set; } = string.Empty;

        public string AvailabilityStatus { get; set; } = "available";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Incident Incident { get; set; } = null!;
    }
}
