
using System.ComponentModel.DataAnnotations;

namespace AISmokingDetection.DTOs
{
    public class CreateEvidenceDto
    {
        [Required]
        public string IncidentId { get; set; } = string.Empty;
        
        public string EvidenceType { get; set; } = string.Empty;

        public string? FileUrl { get; set; }

        public string AvailabilityStatus { get; set; } = "available";

        public string Metadata { get; set; } = "{}";
    }
}
