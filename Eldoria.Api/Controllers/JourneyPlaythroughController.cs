using Eldoria.Api.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/journeys/{journeyId:int}/playthroughs")]
    [ApiController]
    public class JourneyPlaythroughController(
        IJourneyPlaythroughService playthroughService) : ControllerBase
    {
        private readonly IJourneyPlaythroughService _playthroughService =
            playthroughService;

        [HttpPost]
        public async Task<ActionResult<JourneyPlaythroughDto>> Start(
            int journeyId,
            CancellationToken ct)
        {
            var result = await _playthroughService.StartAsync(
                User.GetUserId(),
                journeyId,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(GetActive), new { journeyId }, result.Value);

            return ToActionResult(result.Error);
        }

        [HttpGet("active")]
        public async Task<ActionResult<JourneyPlaythroughDto>> GetActive(
            int journeyId,
            CancellationToken ct)
        {
            var result = await _playthroughService.GetActiveAsync(
                User.GetUserId(),
                journeyId,
                ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpGet]
        public async Task<ActionResult<List<JourneyPlaythroughDto>>> List(
            int journeyId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 100,
            CancellationToken ct = default)
        {
            var result = await _playthroughService.ListAsync(
                User.GetUserId(),
                journeyId,
                skip,
                take,
                ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpPost("{playthroughId:int}/complete")]
        public async Task<ActionResult<JourneyPlaythroughDto>> Complete(
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            var result = await _playthroughService.CompleteAsync(
                User.GetUserId(),
                journeyId,
                playthroughId,
                ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpPost("{playthroughId:int}/deactivate")]
        public async Task<ActionResult<JourneyPlaythroughDto>> Deactivate(
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            var result = await _playthroughService.DeactivateAsync(
                User.GetUserId(),
                journeyId,
                playthroughId,
                ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        private ActionResult ToActionResult(Eldoria.Application.Common.Error error)
        {
            return error.Code switch
            {
                "Journey.NotFound" => NotFound(error),
                "JourneyPlaythrough.NotFound" => NotFound(error),
                "JourneyPlaythrough.ActiveNotFound" => NotFound(error),
                "JourneyPlaythrough.ActiveExists" => Conflict(error),
                "JourneyPlaythrough.NotActive" => Conflict(error),
                _ => BadRequest(error),
            };
        }
    }
}
