using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [ApiController]
    public class SceneProgressController(ISceneProgressService progressService)
        : ControllerBase
    {
        private readonly ISceneProgressService _progressService = progressService;

        [HttpPost("api/v1/journeys/{journeyId:int}/scenes/{sceneId:int}/progress")]
        public async Task<ActionResult<SceneProgressDto>> CreateOrGet(
            int journeyId,
            int sceneId,
            CancellationToken ct)
        {
            var result = await _progressService.CreateOrGetAsync(
                User.GetUserId(), journeyId, sceneId, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpGet("api/v1/journeys/{journeyId:int}/scenes/{sceneId:int}/progress")]
        public async Task<ActionResult<SceneProgressDto>> GetActive(
            int journeyId,
            int sceneId,
            CancellationToken ct)
        {
            var result = await _progressService.GetActiveAsync(
                User.GetUserId(), journeyId, sceneId, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpGet("api/v1/journeys/{journeyId:int}/playthroughs/{playthroughId:int}/scene-progress")]
        public async Task<ActionResult<List<SceneProgressDto>>> List(
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            var result = await _progressService.ListAsync(
                User.GetUserId(), journeyId, playthroughId, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpPatch("api/v1/scene-progress/{sceneProgressId:int}/status")]
        public async Task<ActionResult<SceneProgressDto>> SetStatus(
            int sceneProgressId,
            [FromBody] UpdateSceneProgressStatusRequest request,
            CancellationToken ct)
        {
            var result = await _progressService.SetStatusAsync(
                User.GetUserId(), sceneProgressId, request.Status!.Value, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        private ActionResult ToActionResult(Eldoria.Application.Common.Error error)
        {
            return error.Code switch
            {
                "Scene.NotFound" => NotFound(error),
                "SceneProgress.NotFound" => NotFound(error),
                "JourneyPlaythrough.NotFound" => NotFound(error),
                "JourneyPlaythrough.ActiveNotFound" => NotFound(error),
                "SceneProgress.InvalidTransition" => Conflict(error),
                "SceneProgress.PlaythroughInactive" => Conflict(error),
                _ => BadRequest(error),
            };
        }
    }
}
