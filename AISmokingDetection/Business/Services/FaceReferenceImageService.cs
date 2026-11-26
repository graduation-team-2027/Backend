using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Business.Services
{
    public class FaceReferenceImageService : IFaceReferenceImageService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public FaceReferenceImageService(
            AppDbContext context,
            IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public Task CopyEnrollmentImageToAsync(
            int faceReferenceMetadataId,
            Stream destination,
            CancellationToken cancellationToken)
        {
            return CopyImageAsync(
                faceReferenceMetadataId,
                EnrollmentStatus.Pending,
                destination,
                cancellationToken);
        }

        public Task CopyReadyGalleryImageToAsync(
            int faceReferenceMetadataId,
            Stream destination,
            CancellationToken cancellationToken)
        {
            return CopyImageAsync(
                faceReferenceMetadataId,
                EnrollmentStatus.Ready,
                destination,
                cancellationToken);
        }

        private async Task CopyImageAsync(
            int faceReferenceMetadataId,
            EnrollmentStatus requiredStatus,
            Stream destination,
            CancellationToken cancellationToken)
        {
            if (faceReferenceMetadataId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(faceReferenceMetadataId));
            }

            ArgumentNullException.ThrowIfNull(destination);

            if (!destination.CanWrite)
            {
                throw new ArgumentException(
                    "Destination stream must be writable.",
                    nameof(destination));
            }

            var reference = await _context.FaceReferenceMetadata
                .AsNoTracking()
                .Where(r => r.FaceReferenceMetadataId == faceReferenceMetadataId)
                .Select(r => new
                {
                    r.StorageKey,
                    r.IsActive,
                    r.Status,
                    ParticipantIsActive = r.Participant.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (reference == null)
            {
                throw new KeyNotFoundException(
                    "Face reference was not found.");
            }

            if (!reference.IsActive || !reference.ParticipantIsActive)
            {
                throw new InvalidOperationException(
                    "Face reference or participant is inactive.");
            }

            if (reference.Status != requiredStatus)
            {
                throw new InvalidOperationException(
                    $"Face reference is not in the required state: {requiredStatus}.");
            }

            await _fileStorage.CopyToAsync(
                reference.StorageKey,
                destination,
                cancellationToken);
        }
    }
}
