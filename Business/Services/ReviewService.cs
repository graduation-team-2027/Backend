using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.Data;
using AISmokingDetection.DTOs.ReviewHistory;
using AISmokingDetection.Model;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.API.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewHistoryResponseDto> CreateReviewAsync(
            int incidentId,
            CreateReviewDto dto)
        {
            var incident = await _context.Incidents
                .FirstOrDefaultAsync(i => i.IncidentId == incidentId);

            if (incident == null)
                throw new KeyNotFoundException("Incident not found.");

            if (!Enum.TryParse<IncidentStatus>(
                dto.Decision, true, out var decision))
            {
                throw new ArgumentException("Invalid review decision.");
            }

            if (decision != IncidentStatus.Confirmed &&
                decision != IncidentStatus.Rejected &&
                decision != IncidentStatus.Unresolved)
            {
                throw new ArgumentException("Invalid review decision.");
            }

            if (incident.Status == IncidentStatus.Resolved ||
                incident.Status == IncidentStatus.Rejected)
            {
                throw new InvalidOperationException(
                    "This incident cannot be reviewed again.");
            }

            if (incident.Status == IncidentStatus.Confirmed &&
                decision != IncidentStatus.Rejected)
            {
                throw new InvalidOperationException(
                    "A confirmed incident can only be rejected after new evidence.");
            }

            var review = new ReviewHistory
            {
                IncidentId = incidentId,
                ReviewerId = dto.ReviewerId,
                Decision = decision.ToString(),
                ReviewReason = dto.ReviewReason,
                ReviewedAt = DateTime.UtcNow
            };

            incident.Status = decision;
            incident.ReviewStatus = ReviewStatus.Reviewed;
            incident.UpdatedAt = DateTime.UtcNow;

            _context.ReviewHistories.Add(review);

            await _context.SaveChangesAsync();

            return MapToResponseDto(review);
        }

        public async Task<List<ReviewHistoryResponseDto>>
            GetReviewsByIncidentIdAsync(int incidentId)
        {
            bool incidentExists = await _context.Incidents
                .AnyAsync(i => i.IncidentId == incidentId);

            if (!incidentExists)
                throw new KeyNotFoundException("Incident not found.");

            var reviews = await _context.ReviewHistories
                .Where(r => r.IncidentId == incidentId)
                .OrderByDescending(r => r.ReviewedAt)
                .ToListAsync();

            return reviews.Select(MapToResponseDto).ToList();
        }

        private static ReviewHistoryResponseDto MapToResponseDto(
            ReviewHistory review)
        {
            return new ReviewHistoryResponseDto
            {
                ReviewHistoryId = review.ReviewHistoryId,
                IncidentId = review.IncidentId,
                ReviewerId = review.ReviewerId,
                Decision = review.Decision,
                ReviewReason = review.ReviewReason,
                ReviewedAt = review.ReviewedAt
            };
        }
    }
}