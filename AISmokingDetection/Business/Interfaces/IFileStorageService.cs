namespace AISmokingDetection.Business.Interfaces
{
    public interface IFileStorageService
    {
        Task UploadAsync(
            string storageKey,
            Stream content,
            string contentType,
            CancellationToken cancellationToken);

        Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken);
    }
}
