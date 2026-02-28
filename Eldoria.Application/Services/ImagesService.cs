using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class ImagesService : IImagesService
    {
        private readonly IAzureStorageBlob _azureStorageBlob;

        public ImagesService(IAzureStorageBlob azureStorageBlob)
        {
            _azureStorageBlob = azureStorageBlob;
        }

        public async Task<Result> DeleteImageAsync(string imagePath, CancellationToken ct)
        {
            await _azureStorageBlob.DeletePhotoFromUrl(imagePath);
            return Result.Ok();
        }

        public async Task<Result<ImageUploadResultDto>> SaveImageAsync(IFormFile image, CancellationToken ct)
        {
            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(image);

            var result = new ImageUploadResultDto
            {
                PhotoUrl = photoUrl,
                FileName = fileName
            };

            return Result<ImageUploadResultDto>.Ok(result);
        }
    }
}
