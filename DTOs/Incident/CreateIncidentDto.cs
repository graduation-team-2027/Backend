
namespace AISmokingDetection.DTOs.Incident
{
    public class CreateIncidentDto
    {
            public Guid EventId { get; set; }

            public int CameraId { get; set; }

            public int ParticipantId { get; set; }

            public string EventType { get; set; } = string.Empty;

            public decimal? ConfidenceScore { get; set; }
        }
    }

