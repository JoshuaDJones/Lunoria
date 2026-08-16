using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Eldoria.Application.Services
{
    public class AzureStorageBlob(IConfiguration config) : IAzureStorageBlob
    {
        private readonly string _containerName = config["AzureStorage:ContainerName"] ?? "";
        private readonly BlobServiceClient _blobServiceClient = CreateClient(config);

        public async Task<(string Url, string FileName)> UploadPhoto(IFormFile photo)
        {
            ArgumentNullException.ThrowIfNull(photo);

            if (photo.Length == 0)
                throw new ArgumentException("The uploaded photo is empty.", nameof(photo));

            try
            {
                var containerClient =
                    _blobServiceClient.GetBlobContainerClient(_containerName);

                // UploadAsync does not create the container.
                await containerClient.CreateIfNotExistsAsync(
                    PublicAccessType.Blob);

                var extension = Path.GetExtension(photo.FileName);
                var newFileName = $"{Guid.NewGuid():N}{extension}";
                var blobClient = containerClient.GetBlobClient(newFileName);

                await using var stream = photo.OpenReadStream();

                await blobClient.UploadAsync(
                    stream,
                    new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = photo.ContentType
                        }
                    });

                return (blobClient.Uri.ToString(), newFileName);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Azure status: {ex.Status}");
                Console.WriteLine($"Azure error code: {ex.ErrorCode}");
                Console.WriteLine($"Azure message: {ex.Message}");

                throw;
            }
        }

        private static BlobServiceClient CreateClient(IConfiguration config)
        {
            var account = config["AzureStorage:AccountName"] ?? "";
            var accessKey = config["AzureStorage:AccessKey"] ?? "";
            var credential = new StorageSharedKeyCredential(account, accessKey);
            return new BlobServiceClient(
                new Uri($"https://{account}.blob.core.windows.net"),
                credential);
        }

        public Task<bool> DeletePhotoFromUrl(string? blobUrl)
        {
            // Playthroughs retain copied asset URLs after their source records change.
            // Blob deletion is intentionally disabled so those URLs remain valid.
            return Task.FromResult(true);
        }
    }
}
