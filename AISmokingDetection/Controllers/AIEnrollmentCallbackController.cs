using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Data;
using AISmokingDetection.Business.Interfaces;
using System.Text;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/ai/enrollment-callback")]
    [AllowAnonymous]
    public class AIEnrollmentCallbackController : ControllerBase
    {
        private const string CallbackHeader = "X-AI-Callback-Key";
        private const int MaxFailureReasonLength = 1000;

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IGalleryVersionService _galleryVersionService;

        public AIEnrollmentCallbackController(
            AppDbContext context,
            IConfiguration configuration,
            IGalleryVersionService galleryVersionService)
        {
            _context = context;
            _configuration = configuration;
            _galleryVersionService = galleryVersionService;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveEnrollmentResult(
            [FromBody] EnrollmentCallbackRequest request,
            CancellationToken cancellationToken)
        {
            var configuredKey = _configuration["AI:CallbackKey"];
            var providedKey = Request.Headers[CallbackHeader].ToString();

            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new { message = "AI callback authentication is not configured." });
            }

            if (!IsValidCallbackKey(configuredKey, providedKey))
            {
                return Unauthorized(new
                {
                    message = "Invalid callback authentication."
                });
            }

            if (request.FaceReferenceMetadataId <= 0)
            {
                return BadRequest(new
                {
                    message = "A valid face reference metadata ID is required."
                });
            }

            if (!request.Success &&
                string.IsNullOrWhiteSpace(request.FailureReason))
            {
                return BadRequest(new
                {
                    message = "Failure reason is required when enrollment fails."
                });
            }

            var failureReason = request.Success
                ? null
                : request.FailureReason!.Trim();

            if (failureReason?.Length > MaxFailureReasonLength)
            {
                return BadRequest(new
                {
                    message = "Failure reason cannot exceed 1000 characters."
                });
            }

            var targetStatus = request.Success
                ? EnrollmentStatus.Ready
                : EnrollmentStatus.Failed;

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            var affectedRows = await _context.FaceReferenceMetadata
                .Where(r =>
                    r.FaceReferenceMetadataId == request.FaceReferenceMetadataId &&
                    r.Status == EnrollmentStatus.Pending &&
                    r.IsActive)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(r => r.Status, targetStatus)
                        .SetProperty(r => r.FailureReason, failureReason),
                    cancellationToken);

            if (affectedRows == 1)
            {
                if (targetStatus == EnrollmentStatus.Ready)
                {
                    var participantIsActive = await _context.FaceReferenceMetadata
                        .AnyAsync(
                            r => r.FaceReferenceMetadataId == request.FaceReferenceMetadataId &&
                                 r.IsActive &&
                                 r.Status == EnrollmentStatus.Ready &&
                                 r.Participant.IsActive,
                            cancellationToken);

                    if (participantIsActive)
                    {
                        await _galleryVersionService
                            .IncrementDesiredVersionAsync(cancellationToken);
                    }
                }

                await transaction.CommitAsync(cancellationToken);

                return Ok(new
                {
                    request.FaceReferenceMetadataId,
                    Status = targetStatus,
                    Message = "Enrollment result saved successfully."
                });
            }

            var existingReference = await _context.FaceReferenceMetadata
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    r => r.FaceReferenceMetadataId ==
                         request.FaceReferenceMetadataId,
                    cancellationToken);

            if (existingReference == null)
            {
                return NotFound(new
                {
                    message = "Face reference not found."
                });
            }

            if (!existingReference.IsActive ||
                existingReference.Status == EnrollmentStatus.Revoked)
            {
                return Conflict(new
                {
                    message = "Revoked face references cannot be updated."
                });
            }

            if (existingReference.Status == targetStatus)
            {
                return Ok(new
                {
                    request.FaceReferenceMetadataId,
                    Status = existingReference.Status,
                    Message = "Enrollment result was already processed."
                });
            }

            return Conflict(new
            {
                message = "Enrollment result conflicts with the current status."
            });
        }

        private static bool IsValidCallbackKey(
            string configuredKey,
            string providedKey)
        {
            if (string.IsNullOrWhiteSpace(providedKey))
            {
                return false;
            }

            var expectedBytes = Encoding.UTF8.GetBytes(configuredKey);
            var providedBytes = Encoding.UTF8.GetBytes(providedKey);

            return expectedBytes.Length == providedBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(
                       expectedBytes,
                       providedBytes);
        }
    }

    public class EnrollmentCallbackRequest
    {
        public int FaceReferenceMetadataId { get; set; }

        public bool Success { get; set; }

        public string? FailureReason { get; set; }
    }
}
