using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.DTOs.ReviewHistory;
using Microsoft.AspNetCore.Mvc;

namespace AISmokingDetection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("incident/{incidentId:int}")]
        public async Task<IActionResult> CreateReview(
            int incidentId,
            CreateReviewDto dto)
        {
            var result = await _reviewService.CreateReviewAsync(
                incidentId, dto);

            return CreatedAtAction(
                nameof(GetReviewsByIncidentId),
                new { incidentId },
                result);
        }

        [HttpGet("incident/{incidentId:int}")]
        public async Task<IActionResult> GetReviewsByIncidentId(
            int incidentId)
        {
            var result = await _reviewService
                .GetReviewsByIncidentIdAsync(incidentId);

            return Ok(result);
        }
    }
}