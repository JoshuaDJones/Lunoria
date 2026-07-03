using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Eldoria.Application.Services
{
    public class AzureStorageBlob(IConfiguration config) : IAzureStorageBlob
    {
        private readonly string _containerName = config["AzureStorage:ContainerName"] ?? "";
        private readonly BlobServiceClient _blobServiceClient = CreateClient(config);

        public async Task<(string, string)> UploadPhoto(IFormFile photo)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var ext = Path.GetExtension(photo.FileName);
            var newFileName = $"{Guid.NewGuid()}{ext}";
            var blobClient = containerClient.GetBlobClient(newFileName);

            using var stream = photo.OpenReadStream();
            var result = await blobClient.UploadAsync(stream, true);
            return (blobClient.Uri.ToString(), newFileName);
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
