using AISmokingDetection.Business.Interfaces;

namespace AISmokingDetection.Business.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storagePath;

        public LocalFileStorageService()
        {
            _storagePath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "AISmokingDetection",
                "FaceStorage");
        }

        public async Task UploadAsync(
            string storageKey,
            Stream content,
            string contentType,
            CancellationToken cancellationToken)
        {
            ValidateStorageKey(storageKey);

            Directory.CreateDirectory(_storagePath);

            var filePath = Path.Combine(_storagePath, storageKey);

            await using var output = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                FileOptions.Asynchronous);

            await content.CopyToAsync(output, cancellationToken);
        }

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken)
        {
            ValidateStorageKey(storageKey);

            var filePath = Path.Combine(_storagePath, storageKey);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        private static void ValidateStorageKey(string storageKey)
        {
            if (string.IsNullOrWhiteSpace(storageKey) ||
                storageKey != Path.GetFileName(storageKey))
            {
                throw new ArgumentException("Invalid storage key.");
            }
        }
    }
}
