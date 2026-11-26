using AISmokingDetection.Business.Interfaces;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace AISmokingDetection.Business.Services
{
    public class R2FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public R2FileStorageService(
            IAmazonS3 s3Client,
            IConfiguration configuration)
        {
            _s3Client = s3Client;

            _bucketName = configuration["CloudflareR2:BucketName"]
                ?? throw new InvalidOperationException(
                    "Cloudflare R2 bucket name is missing.");
        }

        public async Task UploadAsync(
            string storageKey,
            Stream content,
            string contentType,
            CancellationToken cancellationToken)
        {
            ValidateStorageKey(storageKey);
            ArgumentNullException.ThrowIfNull(content);

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = storageKey,
                InputStream = content,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(
                request,
                cancellationToken);
        }

        public async Task CopyToAsync(
            string storageKey,
            Stream destination,
            CancellationToken cancellationToken)
        {
            ValidateStorageKey(storageKey);
            ValidateDestination(destination);

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = storageKey
            };

            using var response = await _s3Client.GetObjectAsync(
                request,
                cancellationToken);

            await response.ResponseStream.CopyToAsync(
                destination,
                cancellationToken);
        }

        public async Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken)
        {
            ValidateStorageKey(storageKey);

            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = storageKey
            };

            await _s3Client.DeleteObjectAsync(
                request,
                cancellationToken);
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
