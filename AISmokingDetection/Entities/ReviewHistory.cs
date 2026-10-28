
namespace AISmokingDetection.Model
{
    public class ReviewHistory
    {
        public int ReviewHistoryId { get; set; }

        public required string IncidentId { get; set; }

       
        public string? ReviewReason { get; set; }

        public int ReviewerId { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Incident? Incident { get; set; }
    }
}
