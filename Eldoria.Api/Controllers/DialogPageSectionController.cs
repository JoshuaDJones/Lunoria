using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Eldoria.Api.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DialogPageSectionController(IDialogPageSectionService dialogPageSectionService) : ControllerBase
    {
        private readonly IDialogPageSectionService _dialogPageSectionService = dialogPageSectionService;

        [HttpPost("{dialogPageId:int}")]
        public async Task<IActionResult> Create(int dialogPageId, [FromBody] CreateDialogPageSectionRequest req, CancellationToken ct)
        {
            var result = await _dialogPageSectionService.CreateDialogPageSectionAsync(User.GetUserId(), dialogPageId, req.CharacterId, req.OrderNum!.Value, req.ReadingText, req.IsNarrator, ct);

            if (result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "DialogPage.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPatch("{dialogPageSectionId:int}")]
        public async Task<IActionResult> Update(int dialogPageSectionId, [FromBody] UpdateDialogPageSectionRequest req, CancellationToken ct)
        {
            var result = await _dialogPageSectionService.EditDialogPageSectionAsync(User.GetUserId(), dialogPageSectionId, req.CharacterId, req.OrderNum, req.ReadingText, req.IsNarrator, ct);

            if (result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "DialogPageSection.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{dialogPageSectionId:int}")]
        public async Task<IActionResult> Delete(int dialogPageSectionId, CancellationToken ct)
        {
            var result = await _dialogPageSectionService.DeleteDialogPageSectionAsync(dialogPageSectionId, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "DialogPageSection.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
