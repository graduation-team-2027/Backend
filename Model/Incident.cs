

namespace AISmokingDetection.Model
{
    public enum IncidentStatus
    {
        Suspected,
        Unresolved,
        Confirmed,
        Rejected,
        Resolved
    }
    public enum IdentityStatus
    {
        MatchedCandidate,
        Unknown,
        InsufficientQuality
    }
    public enum ReviewStatus
    {
        Pending,
        InReview,
        Reviewed
    }
    public class Incident
    {
        public int IncidentId { get; set; } 
        public Guid EventId { get; set; }
        public int CameraId { get; set; }
        public int ParticipantId { get; set; }
        public string EventType { get; set; } = string.Empty; // cigarette, vape, uncertain
        public IncidentStatus Status { get; set; } = IncidentStatus.Suspected;
        public IdentityStatus IdentityStatus { get; set; } = IdentityStatus.Unknown;
        public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public decimal? ConfidenceScore { get; set; }
       

        // Navigation Property
        public ICollection<ProcessingJob> ProcessingJobs { get; set; }
       = new List<ProcessingJob>();
        public  ICollection<Evidence> Evidences { get; set; }= new List<Evidence>();
        public  ICollection<ReviewHistory> ReviewHistories { get; set; }
        = new List<ReviewHistory>();

    }
}
