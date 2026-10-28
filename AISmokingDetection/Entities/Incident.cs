using System.Security.Policy;

namespace AISmokingDetection.Model
{
    public class Incident
    {
        public string IncidentId { get; set; } = string.Empty;
        public Guid EventId { get; set; } 
        public string EventType { get; set; } = string.Empty; // cigarette, vape, uncertain
        public string Status { get; set; } = "suspected"; // suspected, confirmed, rejected, unresolved, resolved
        public string IdentityStatus { get; set; } = "unknown"; // matched_candidate, unknown, insufficient_quality
       
        
        public decimal? ConfidenceScore { get; set; }
        public string DataOrigin { get; set; } = "fixture";
        public string? ModelVersion { get; set; }
        public string? Source { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
       public List<Evidence> Evidence { get; set; } = new();

    }
}
