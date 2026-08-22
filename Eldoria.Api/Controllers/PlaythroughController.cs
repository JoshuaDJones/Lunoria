using Eldoria.Api.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/journeys/{journeyId:int}/playthroughs")]
    [ApiController]
    public class PlaythroughController(IPlaythroughService playthroughService) : ControllerBase
    {
        private readonly IPlaythroughService _playthroughService = playthroughService;

        [HttpGet]
        public async Task<ActionResult<List<PlaythroughSummaryDto>>> GetForJourney(
            int journeyId,
            CancellationToken ct)
        {
            var result = await _playthroughService.GetForJourneyAsync(
                User.GetUserId(),
                journeyId,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code switch
            {
                "Journey.NotFound" => NotFound(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        public async Task<ActionResult<PlaythroughCreatedDto>> Start(
            int journeyId,
            CancellationToken ct)
        {
            var result = await _playthroughService.StartAsync(
                User.GetUserId(),
                journeyId,
                ct);

            if (result.Success)
                return StatusCode(StatusCodes.Status201Created, result.Value);

            return result.Error.Code switch
            {
                "Journey.NotFound" => NotFound(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpGet("~/api/v1/playthroughs/{playthroughId:int}")]
        public async Task<ActionResult<PlaythroughDetailsDto>> Get(
            int playthroughId,
            CancellationToken ct)
        {
            var result = await _playthroughService.GetAsync(
                User.GetUserId(),
                playthroughId,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code switch
            {
                "Playthrough.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
