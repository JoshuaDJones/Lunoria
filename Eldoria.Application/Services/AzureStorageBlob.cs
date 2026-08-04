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

        public async Task<bool> DeletePhotoFromUrl(string blobUrl)
        {
            try
            {
                var uri = new Uri(blobUrl);
                var blobName = uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf('/') + 1);
                var containerName = uri.Segments[1].TrimEnd('/');

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var result = await blobClient.DeleteIfExistsAsync();
                return result.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting blob from URL: {ex.Message}");
                return false;
            }
        }
    }
}
