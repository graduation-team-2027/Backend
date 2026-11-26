using System.Security.Cryptography;
using System.Text;
using AISmokingDetection.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/ai/face-references")]
    [AllowAnonymous]
    public class AIFaceReferencesController : ControllerBase
    {
        private readonly IFaceReferenceImageService _imageService;
        private readonly IConfiguration _configuration;

        public AIFaceReferencesController(
            IFaceReferenceImageService imageService,
            IConfiguration configuration)
        {
            _imageService = imageService;
            _configuration = configuration;
        }

        [HttpGet("{id:int}/enrollment-image")]
        public Task<IActionResult> GetEnrollmentImage(
            int id,
            CancellationToken cancellationToken)
        {
            return GetImageAsync(id, true, cancellationToken);
        }

        [HttpGet("{id:int}/gallery-image")]
        public Task<IActionResult> GetGalleryImage(
            int id,
            CancellationToken cancellationToken)
        {
            return GetImageAsync(id, false, cancellationToken);
        }

        private async Task<IActionResult> GetImageAsync(
            int id,
            bool enrollment,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid face reference ID." });

            var configuredKey = _configuration["AI:ServiceKey"];

            if (string.IsNullOrWhiteSpace(configuredKey))
            {
                return StatusCode(503, new
                {
                    message = "AI service authentication is not configured."
                });
            }

            var providedKey = Request.Headers["X-AI-Service-Key"].ToString();

            if (string.IsNullOrWhiteSpace(providedKey))
                return Unauthorized();

            var expectedBytes = Encoding.UTF8.GetBytes(configuredKey);
            var providedBytes = Encoding.UTF8.GetBytes(providedKey);

            if (!CryptographicOperations.FixedTimeEquals(
                expectedBytes,
                providedBytes))
            {
                return Unauthorized();
            }

            var image = new MemoryStream();

            try
            {
                if (enrollment)
                {
                    await _imageService.CopyEnrollmentImageToAsync(
                        id,
                        image,
                        cancellationToken);
                }
                else
                {
                    await _imageService.CopyReadyGalleryImageToAsync(
                        id,
                        image,
                        cancellationToken);
                }

                var bytes = image.ToArray();
                image.Dispose();

                var contentType = GetImageContentType(bytes);

                if (contentType == null)
                {
                    return StatusCode(415, new
                    {
                        message = "Unsupported image format."
                    });
                }

                return File(bytes, contentType);
            }
            catch (KeyNotFoundException)
            {
                image.Dispose();
                return NotFound(new
                {
                    message = "Face reference was not found."
                });
            }
            catch (InvalidOperationException ex)
            {
                image.Dispose();
                return Conflict(new { message = ex.Message });
            }
            catch (FileNotFoundException)
            {
                image.Dispose();
                return NotFound(new
                {
                    message = "Stored face reference image was not found."
                });
            }
            catch
            {
                image.Dispose();
                throw;
            }
        }

        private static string? GetImageContentType(byte[] bytes)
        {
            if (bytes.Length >= 3 &&
                bytes[0] == 0xFF &&
                bytes[1] == 0xD8 &&
                bytes[2] == 0xFF)
            {
                return "image/jpeg";
            }

            if (bytes.Length >= 8 &&
                bytes[0] == 0x89 &&
                bytes[1] == 0x50 &&
                bytes[2] == 0x4E &&
                bytes[3] == 0x47 &&
                bytes[4] == 0x0D &&
                bytes[5] == 0x0A &&
                bytes[6] == 0x1A &&
                bytes[7] == 0x0A)
            {
                return "image/png";
            }

            return null;
        }
    }
}
