namespace AISmokingDetection.Model

{
    public class Evidence
    {
        public Guid EvidenceId { get; set; }
        public  int IncidentId { get; set; }  // Foreign Key to Incident
        public string EvidenceType { get; set; } = string.Empty; // image, video
        public string? StorageReference { get; set; } // Map from Uri/FileUrl
        public string AvailabilityStatus { get; set; } = "available";
       
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Incident? Incident { get; set; }

    }
}
