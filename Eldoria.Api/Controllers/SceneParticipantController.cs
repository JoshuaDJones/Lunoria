using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/scene-progress/{sceneProgressId:int}")]
    [ApiController]
    public class SceneParticipantController(
        ISceneParticipantService participantService) : ControllerBase
    {
        private readonly ISceneParticipantService _participantService = participantService;

        [HttpPost("participants")]
        public async Task<ActionResult<SceneParticipantDto>> AddParticipant(
            int sceneProgressId,
            [FromBody] AddSceneParticipantRequest request,
            CancellationToken ct)
        {
            var result = await _participantService.AddAsync(
                User.GetUserId(),
                sceneProgressId,
                request.JourneyCharacterId,
                request.SceneCharacterId,
                ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpDelete("participants/{participantId:int}")]
        public async Task<IActionResult> RemoveParticipant(
            int sceneProgressId,
            int participantId,
            CancellationToken ct)
        {
            var result = await _participantService.RemoveAsync(
                User.GetUserId(), sceneProgressId, participantId, ct);

            return result.Success ? NoContent() : ToActionResult(result.Error);
        }

        [HttpPost("turns")]
        public async Task<ActionResult<SceneParticipantTurnDto>> AddTurn(
            int sceneProgressId,
            [FromBody] AddSceneParticipantTurnRequest request,
            CancellationToken ct)
        {
            var result = await _participantService.AddTurnAsync(
                User.GetUserId(),
                sceneProgressId,
                request.SceneParticipantId!.Value,
                request.TurnPosition!.Value,
                ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpPut("turns/reorder")]
        public async Task<ActionResult<List<SceneParticipantTurnDto>>> ReorderTurns(
            int sceneProgressId,
            [FromBody] ReorderSceneParticipantTurnsRequest request,
            CancellationToken ct)
        {
            var positions = request.Turns
                .Select(turn => (turn.TurnId!.Value, turn.TurnPosition!.Value))
                .ToList();

            var result = await _participantService.ReorderTurnsAsync(
                User.GetUserId(), sceneProgressId, positions, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpDelete("turns/{turnId:int}")]
        public async Task<IActionResult> RemoveTurn(
            int sceneProgressId,
            int turnId,
            CancellationToken ct)
        {
            var result = await _participantService.RemoveTurnAsync(
                User.GetUserId(), sceneProgressId, turnId, ct);

            return result.Success ? NoContent() : ToActionResult(result.Error);
        }

        private ActionResult ToActionResult(Eldoria.Application.Common.Error error)
        {
            return error.Code switch
            {
                "SceneProgress.NotFound" => NotFound(error),
                "JourneyCharacter.NotFound" => NotFound(error),
                "SceneCharacter.NotFound" => NotFound(error),
                "SceneParticipant.NotFound" => NotFound(error),
                "SceneParticipantTurn.NotFound" => NotFound(error),
                "SceneParticipant.Duplicate" => Conflict(error),
                "SceneParticipant.HasTurns" => Conflict(error),
                "SceneParticipantTurn.PositionConflict" => Conflict(error),
                "SceneProgress.PlaythroughInactive" => Conflict(error),
                "SceneParticipant.InvalidCharacter" => BadRequest(error),
                "SceneParticipantTurn.InvalidOrder" => BadRequest(error),
                _ => BadRequest(error),
            };
        }
    }
}
