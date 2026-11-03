namespace AISmokingDetection.Model
{
    public class ReviewHistory
    {
        public int ReviewHistoryId { get; set; }
        public int ReviewerId { get; set; }
        public int IncidentId { get; set; }

       
        public string? ReviewReason { get; set; }
        public string Decision { get; set; } = string.Empty;
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Incident? Incident { get; set; }
      
       
    }
}
