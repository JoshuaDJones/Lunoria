using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyIntroPageController(IJourneyIntroPageService journeyIntroPageService) : ControllerBase
    {
        private readonly IJourneyIntroPageService _journeyIntroPageService = journeyIntroPageService;

        [HttpGet]
        public async Task<ActionResult<List<JourneyIntroPageDto>>> List(
            [FromQuery] int journeyId,
            CancellationToken ct)
        {
            var result = await _journeyIntroPageService.ListAsync(
                User.GetUserId(),
                journeyId,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
            [FromForm] CreateJourneyIntroPageRequest req,
            CancellationToken ct)
        {
            var result = await _journeyIntroPageService.CreateAsync(
                User.GetUserId(),
                req.JourneyId!.Value,
                req.Type!.Value,
                req.Config,
                req.Image,
                ct);

            if (result.Success)
                return CreatedAtAction(
                    nameof(List),
                    new { journeyId = req.JourneyId.Value },
                    result.Value);

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdateJourneyIntroPageRequest req,
            CancellationToken ct)
        {
            var result = await _journeyIntroPageService.UpdateAsync(
                User.GetUserId(),
                req.JourneyId!.Value,
                id,
                req.Type!.Value,
                req.Config,
                req.Image,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "JourneyIntroPage.NotFound" => NotFound(result.Error),
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromQuery] int journeyId,
            CancellationToken ct)
        {
            var result = await _journeyIntroPageService.DeleteAsync(
                User.GetUserId(),
                journeyId,
                id,
                ct);

            if (result.Success)
                return NoContent();

            return result.Error?.Code switch
            {
                "JourneyIntroPage.NotFound" => BadRequest(result.Error),
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPut("order")]
        public async Task<IActionResult> Reorder(
            [FromQuery] int journeyId,
            [FromBody] ReorderJourneyIntroPagesRequest req,
            CancellationToken ct)
        {
            var pages = req.Pages
                .Select(page => (PageId: page.Id, page.SortOrder))
                .ToList();
            var result = await _journeyIntroPageService.ReorderAsync(
                User.GetUserId(),
                journeyId,
                pages,
                ct);

            if (result.Success)
                return NoContent();

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }
    }
}
