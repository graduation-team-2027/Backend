
namespace AISmokingDetection.DTOs
{
    public class CreateIncidentDto
    {
        public Guid EventId { get; set; } 

        public string EventType { get; set; } = string.Empty;


        public decimal? ConfidenceScore { get; set; }

        public string? ModelVersion { get; set; }

        public string? Source { get; set; }
        
    }
}
