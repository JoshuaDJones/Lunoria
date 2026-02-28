using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface IImagesService
    {
        Task<Result<ImageUploadResultDto>> SaveImageAsync(IFormFile image, CancellationToken ct);
        Task<Result> DeleteImageAsync(string imagePath, CancellationToken ct);
    }
}
