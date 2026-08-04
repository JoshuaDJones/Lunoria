using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface IAzureStorageBlob
    {
        Task<(string Url, string FileName)> UploadPhoto(IFormFile photo);
        Task<bool> DeletePhotoFromUrl(string url);
    }
}
