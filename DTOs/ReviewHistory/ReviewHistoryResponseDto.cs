namespace AISmokingDetection.DTOs.ReviewHistory
{
    public class ReviewHistoryResponseDto
    {
        public int ReviewHistoryId { get; set; }

        public int IncidentId { get; set; }

        public int ReviewerId { get; set; }

        public string Decision { get; set; } = string.Empty;

        public string? ReviewReason { get; set; }

        public DateTime ReviewedAt { get; set; }
    }
}