namespace AISmokingDetection.Model

{
    public class Evidence
    {
        public Guid EvidenceId { get; set; }
        public  string IncidentId { get; set; } = string.Empty; // Foreign Key to Incident
        public string EvidenceType { get; set; } = string.Empty; // image, video
        public string? FileUrl { get; set; } // Map from Uri/FileUrl
        public string AvailabilityStatus { get; set; } = "available";
        public string Metadata { get; set; } = "{}"; // JSON string payload
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Incident? Incident { get; set; }

    }
}
