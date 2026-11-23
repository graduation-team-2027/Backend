
using AISmokingDetection.Entities;

namespace AISmokingDetection.Model
{
    public class ReviewHistory
    {
        public int ReviewHistoryId { get; set; }

        public int IncidentId { get; set; }

        public int ReviewerId { get; set; }

        public string Decision { get; set; } = string.Empty;

        public string? ReviewReason { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        public Incident Incident { get; set; } = null!;

        public Staff Reviewer { get; set; } = null!;
    }
}
