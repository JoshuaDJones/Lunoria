using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Requests
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; } = null!;
        public string? Name { get; set; }
    }
}
