
namespace AISmokingDetection.DTOs
{
    public class ReviewIncidentDto
    {
        public string NewStatus { get; set; } = string.Empty;

        public string ReviewReason { get; set; } = string.Empty;

        public int ReviewerId { get; set; }
    }
}
