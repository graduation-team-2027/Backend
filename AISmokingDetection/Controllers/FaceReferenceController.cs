
using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FaceReferenceController : ControllerBase
    {
        private const long MaxFileSize = 5 * 1024 * 1024;

        private readonly AppDbContext _context;
        private readonly string _storagePath;

        public FaceReferenceController(AppDbContext context)
        {
            _context = context;

            _storagePath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "AISmokingDetection",
                "FaceStorage");
        }

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
                return BadRequest(new
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

            Directory.CreateDirectory(_storagePath);

            var storageKey = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(_storagePath, storageKey);

            try
            {
                await using (var output = new FileStream(
                    filePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    81920,
                    FileOptions.Asynchronous))
                {
                    await image.CopyToAsync(output, cancellationToken);
                }

                var metadata = new FaceReferenceMetadata
                {
                    ParticipantId = participantId,
                    StorageKey = storageKey,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.FaceReferenceMetadata.Add(metadata);

                await _context.SaveChangesAsync(cancellationToken);

                return Created(
                    $"/api/FaceReference/{metadata.FaceReferenceMetadataId}",
                    new
                    {
                        metadata.FaceReferenceMetadataId,
                        metadata.ParticipantId,
                        metadata.StorageKey,
                        metadata.CreatedAt
                    });
            }
            catch
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                throw;
            }
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
