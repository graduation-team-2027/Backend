namespace AISmokingDetection.Business.Interfaces
{
    public interface IGalleryVersionService
    {
        Task IncrementDesiredVersionAsync(
            CancellationToken cancellationToken);
    }
}
