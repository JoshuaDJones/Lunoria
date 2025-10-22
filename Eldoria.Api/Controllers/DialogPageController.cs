using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DialogPageController : ControllerBase
    {
        private readonly IDialogPageService _dialogPageService;

        public DialogPageController(IDialogPageService dialogPageService)
        {
            _dialogPageService = dialogPageService;
        }

        [HttpPost("{sceneDialogId:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(int sceneDialogId, [FromForm] CreateDialogPageRequest req, CancellationToken ct)
        {
            var result = await _dialogPageService.CreateDialogPageAsync(sceneDialogId, req.OrderNum, req.Photo, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "SceneDialog.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPatch("{dialogPageId:int}")]
        public async Task<IActionResult> Update(int dialogPageId, [FromBody] UpdateDialogPageRequest req, CancellationToken ct)
        {
            var result = await _dialogPageService.EditDialogPageAsync(dialogPageId, req.OrderNum, req.Photo, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "DialogPage.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
