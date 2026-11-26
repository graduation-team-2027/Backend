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
            ArgumentNullException.ThrowIfNull(content);

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

        public async Task CopyToAsync(
            string storageKey,
            Stream destination,
            CancellationToken cancellationToken)
        {
            ValidateStorageKey(storageKey);
            ValidateDestination(destination);

            var filePath = Path.Combine(_storagePath, storageKey);

            await using var input = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                81920,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            await input.CopyToAsync(destination, cancellationToken);
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

        private static void ValidateDestination(Stream destination)
        {
            ArgumentNullException.ThrowIfNull(destination);

            if (!destination.CanWrite)
            {
                throw new ArgumentException(
                    "Destination stream must be writable.",
                    nameof(destination));
            }
        }

        private static void ValidateStorageKey(string storageKey)
        {
            if (string.IsNullOrWhiteSpace(storageKey) ||
                storageKey != Path.GetFileName(storageKey) ||
                storageKey.Contains('/') ||
                storageKey.Contains('\\'))
            {
                throw new ArgumentException("Invalid storage key.");
            }
        }
    }
}
