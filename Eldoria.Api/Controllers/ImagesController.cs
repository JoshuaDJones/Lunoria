using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _imagesService;

        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        [HttpPost]
        public async Task<ActionResult<ImageUploadResultDto>> UploadImage([FromForm] IFormFile file, [FromForm] string? name, CancellationToken ct)
        {
            var result = await _imagesService.SaveImageAsync(file, ct);

            if (result.Success)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteImage([FromQuery] string imagePath, CancellationToken ct)
        {
            var result = await _imagesService.DeleteImageAsync(imagePath, ct);

            if (result.Success)
                return NoContent();

            return BadRequest(result.Error);
        }
    }
}
