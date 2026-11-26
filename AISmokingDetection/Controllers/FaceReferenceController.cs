using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FaceReferenceController : ControllerBase
    {
        private const long MaxFileSize = 5 * 1024 * 1024;

        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly IGalleryVersionService _galleryVersionService;

        public FaceReferenceController(
            AppDbContext context,
            IFileStorageService fileStorage,
            IGalleryVersionService galleryVersionService)
        {
            _context = context;
            _fileStorage = fileStorage;
            _galleryVersionService = galleryVersionService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("{participantId:int}")]
        [RequestSizeLimit(MaxFileSize + 1024 * 1024)]
        public async Task<IActionResult> UploadFaceReference(
            int participantId,
            IFormFile? image,
            CancellationToken cancellationToken)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new
                {
                    message = "An image file is required."
                });
            }

            if (image.Length > MaxFileSize)
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge, new
                {
                    message = "Maximum image size is 5 MB."
                });
            }

            var participant = await _context.Participants
                .AsNoTracking()
                .AnyAsync(
                    p => p.ParticipantId == participantId && p.IsActive,
                    cancellationToken);

            if (!participant)
            {
                return NotFound(new
                {
                    message = "Active participant not found."
                });
            }

            var extension = Path.GetExtension(image.FileName)
                .ToLowerInvariant();

            if (extension != ".jpg" &&
                extension != ".jpeg" &&
                extension != ".png")
            {
                return BadRequest(new
                {
                    message = "Only JPG and PNG images are allowed."
                });
            }

            var header = new byte[8];

            await using (var input = image.OpenReadStream())
            {
                var bytesRead = await input.ReadAsync(
                    header.AsMemory(0, header.Length),
                    cancellationToken);

                if (!IsValidImageHeader(header, bytesRead))
                {
                    return BadRequest(new
                    {
                        message = "The uploaded file is not a valid JPG or PNG image."
                    });
                }
            }

            var storageKey = $"{Guid.NewGuid():N}{extension}";
            var storageUploaded = false;

            try
            {
                var contentType = extension == ".png"
                    ? "image/png"
                    : "image/jpeg";

                await using (var content = image.OpenReadStream())
                {
                    await _fileStorage.UploadAsync(
                        storageKey,
                        content,
                        contentType,
                        cancellationToken);
                }

                storageUploaded = true;

                var metadata = new FaceReferenceMetadata
                {
                    ParticipantId = participantId,
                    StorageKey = storageKey,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Status = EnrollmentStatus.Pending,
                    FailureReason = null
                };

                _context.FaceReferenceMetadata.Add(metadata);

                await _context.SaveChangesAsync(cancellationToken);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new
                    {
                        metadata.FaceReferenceMetadataId,
                        metadata.ParticipantId,
                        metadata.CreatedAt,
                        metadata.IsActive,
                        metadata.Status
                    });
            }
            catch
            {
                if (storageUploaded)
                {
                    try
                    {
                        await _fileStorage.DeleteAsync(
                            storageKey,
                            CancellationToken.None);
                    }
                    catch
                    {
                    }
                }

                throw;
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("participant/{participantId:int}")]
        public async Task<IActionResult> GetParticipantFaceReferences(
            int participantId,
            CancellationToken cancellationToken)
        {
            var participantExists = await _context.Participants
                .AsNoTracking()
                .AnyAsync(
                    p => p.ParticipantId == participantId,
                    cancellationToken);

            if (!participantExists)
            {
                return NotFound(new
                {
                    message = "Participant not found."
                });
            }

            var references = await _context.FaceReferenceMetadata
                .AsNoTracking()
                .Where(r => r.ParticipantId == participantId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.FaceReferenceMetadataId,
                    r.ParticipantId,
                    r.CreatedAt,
                    r.IsActive,
                    r.Status,
                    r.FailureReason
                })
                .ToListAsync(cancellationToken);

            return Ok(references);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("{id:int}/revoke")]
        public async Task<IActionResult> RevokeFaceReference(
            int id,
            CancellationToken cancellationToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            var reference = await _context.FaceReferenceMetadata
                .Include(r => r.Participant)
                .FirstOrDefaultAsync(
                    r => r.FaceReferenceMetadataId == id,
                    cancellationToken);

            if (reference == null)
            {
                return NotFound(new
                {
                    message = "Face reference not found."
                });
            }

            if (reference.Status == EnrollmentStatus.Revoked ||
                !reference.IsActive)
            {
                return Conflict(new
                {
                    message = "Face reference has already been revoked."
                });
            }

            var affectsGallery =
                reference.IsActive &&
                reference.Status == EnrollmentStatus.Ready &&
                reference.Participant.IsActive;

            reference.IsActive = false;
            reference.Status = EnrollmentStatus.Revoked;

            if (affectsGallery)
            {
                await _galleryVersionService
                    .IncrementDesiredVersionAsync(cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Ok(new
            {
                reference.FaceReferenceMetadataId,
                reference.ParticipantId,
                reference.CreatedAt,
                reference.IsActive,
                reference.Status,
                message = "Face reference revoked successfully. Stored file was retained."
            });
        }

        private static bool IsValidImageHeader(
            byte[] header,
            int bytesRead)
        {
            var isJpeg =
                bytesRead >= 3 &&
                header[0] == 0xFF &&
                header[1] == 0xD8 &&
                header[2] == 0xFF;

            var isPng =
                bytesRead >= 8 &&
                header[0] == 0x89 &&
                header[1] == 0x50 &&
                header[2] == 0x4E &&
                header[3] == 0x47 &&
                header[4] == 0x0D &&
                header[5] == 0x0A &&
                header[6] == 0x1A &&
                header[7] == 0x0A;

            return isJpeg || isPng;
        }
    }
}
