using AISmokingDetection.DTOs.ReviewHistory;

namespace AISmokingDetection.Business.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewHistoryResponseDto> CreateReviewAsync(
            int incidentId,
            CreateReviewDto dto);

        Task<List<ReviewHistoryResponseDto>> GetReviewsByIncidentIdAsync(
            int incidentId);
    }
}