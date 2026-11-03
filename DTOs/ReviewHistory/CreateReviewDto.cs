
namespace AISmokingDetection.DTOs.ReviewHistory
{
    public class CreateReviewDto
    {
        public int ReviewerId { get; set; }

        public string Decision { get; set; } = string.Empty;

        public string? ReviewReason { get; set; }
    }
}