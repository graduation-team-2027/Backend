using System.Threading;
using System.Threading.Tasks;

namespace AISmokingDetection.Business.Interfaces
{
    public interface IFaceReferenceImageService
    {
        Task CopyEnrollmentImageToAsync(
            int faceReferenceMetadataId,
            Stream destination,
            CancellationToken cancellationToken);

        Task CopyReadyGalleryImageToAsync(
            int faceReferenceMetadataId,
            Stream destination,
            CancellationToken cancellationToken);
    }
}
