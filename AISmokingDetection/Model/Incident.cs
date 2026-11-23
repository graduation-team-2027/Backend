
using AISmokingDetection.Entities;

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

        public int? ParticipantId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public IncidentStatus Status { get; set; } = IncidentStatus.Suspected;

        public IdentityStatus IdentityStatus { get; set; } = IdentityStatus.Unknown;

        public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.Pending;

        public decimal? ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Camera Camera { get; set; } = null!;

        public Participant? Participant { get; set; }

        public ICollection<Evidence> Evidences { get; set; } = new List<Evidence>();

        public ICollection<ProcessingJob> ProcessingJobs { get; set; } = new List<ProcessingJob>();

        public ICollection<ReviewHistory> ReviewHistories { get; set; } = new List<ReviewHistory>();
    }
}
