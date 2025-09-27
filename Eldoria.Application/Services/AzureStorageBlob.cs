using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class AzureStorageBlob : IAzureStorageBlob
    {
        private readonly string _storageAccount = "jdjstorageaccount";
        private readonly string _recipeContainerName = "recipe-photos";
        private readonly string _accessKey = "j5SNLNjk5raMzuh9etqJ/2RqsaymVNEhAXcaaZxaCMaGP1bhxfBptBufkaE9EEFhFNTziCdlLhSG+ASt2PROkA==";
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorageBlob()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _accessKey);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";

            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }

        public async Task<(string, string)> UploadPhoto(IFormFile photo)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_recipeContainerName);
            var ext = Path.GetExtension(photo.FileName);
            var newFileName = $"{Guid.NewGuid()}{ext}";
            var blobClient = containerClient.GetBlobClient(newFileName);

            using var stream = photo.OpenReadStream();
            var result = await blobClient.UploadAsync(stream, true);
            return (blobClient.Uri.ToString(), newFileName);
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
