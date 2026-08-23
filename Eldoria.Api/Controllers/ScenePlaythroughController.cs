using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Api.PlaythroughRealtime;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers;

[Route("api/v1/playthroughs/{playthroughId:int}/scenes/{sceneId:int}")]
[ApiController]
public sealed class ScenePlaythroughController(
    IScenePlaythroughService scenePlaythroughService,
    IPlaythroughRealtimeNotifier realtimeNotifier) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ScenePlaythroughDetailsDto>> Get(
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.GetAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            ct);

        if (result.Success)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "ScenePlaythrough.NotFound" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.StartAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            ct);

        return await CompleteMutationAsync(result, playthroughId, "SceneStarted", ct);
    }

    [HttpPost("participants/scene-characters/{scenePlaythroughCharacterId:int}")]
    public async Task<IActionResult> AddSceneCharacterInstance(
        int playthroughId,
        int sceneId,
        int scenePlaythroughCharacterId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.AddSceneCharacterInstanceAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            scenePlaythroughCharacterId,
            ct);

        return await CompleteMutationAsync(
            result, playthroughId, "SceneCharacterAdded", ct);
    }

    [HttpPost("participants/journey-characters/{journeyPlaythroughCharacterId:int}/activate")]
    public async Task<IActionResult> ActivateJourneyCharacter(
        int playthroughId,
        int sceneId,
        int journeyPlaythroughCharacterId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.ActivateJourneyCharacterAsync(
            User.GetUserId(), playthroughId, sceneId,
            journeyPlaythroughCharacterId, ct);
        return await CompleteMutationAsync(
            result, playthroughId, "JourneyCharacterActivated", ct);
    }

    [HttpPost("participants/playthrough-characters/{playthroughCharacterId:int}")]
    public async Task<IActionResult> AddPlaythroughCharacter(
        int playthroughId,
        int sceneId,
        int playthroughCharacterId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.AddPlaythroughCharacterAsync(
            User.GetUserId(), playthroughId, sceneId, playthroughCharacterId, ct);
        return await CompleteMutationAsync(
            result, playthroughId, "SceneCharacterAdded", ct);
    }

    [HttpPut("participants/{participantId:int}/stats")]
    public async Task<IActionResult> UpdateParticipantStats(
        int playthroughId,
        int sceneId,
        int participantId,
        [FromBody] UpdateSceneParticipantStatsRequest request,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.UpdateParticipantStatsAsync(
            User.GetUserId(), playthroughId, sceneId, participantId,
            request.ToDto(), ct);
        return await CompleteMutationAsync(
            result, playthroughId, "ParticipantStatsUpdated", ct);
    }

    [HttpPost("participants/{participantId:int}/movement")]
    public async Task<ActionResult<SceneMovementResultDto>> RecordMovement(
        int playthroughId,
        int sceneId,
        int participantId,
        [FromBody] RecordSceneMovementRequest request,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.RecordMovementAsync(
            User.GetUserId(), playthroughId, sceneId, participantId,
            request.Roll, ct);

        if (result.Success)
        {
            await realtimeNotifier.NotifyUpdatedAsync(
                playthroughId, "MovementRecorded", ct);
            return Ok(result.Value);
        }

        return result.Error.Code switch
        {
            "ScenePlaythrough.NotFound" => NotFound(result.Error),
            "ScenePlaythrough.ParticipantNotFound" => NotFound(result.Error),
            "ScenePlaythrough.NotInProgress" => Conflict(result.Error),
            "ScenePlaythrough.NotCurrentTurn" => Conflict(result.Error),
            "Playthrough.Completed" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost("participants/{participantId:int}/forfeit-action")]
    public async Task<IActionResult> ForfeitAction(
        int playthroughId,
        int sceneId,
        int participantId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.ForfeitActionAsync(
            User.GetUserId(), playthroughId, sceneId, participantId, ct);
        return await CompleteMutationAsync(
            result, playthroughId, "ActionForfeited", ct);
    }

    private async Task<IActionResult> CompleteMutationAsync(
        Result result,
        int playthroughId,
        string reason,
        CancellationToken ct)
    {
        if (!result.Success)
            return ToError(result.Error);

        await realtimeNotifier.NotifyUpdatedAsync(playthroughId, reason, ct);
        return NoContent();
    }

    private IActionResult ToError(Error error) => error.Code switch
    {
        "ScenePlaythrough.NotFound" => NotFound(error),
        "ScenePlaythrough.AlreadyStarted" => Conflict(error),
        "ScenePlaythrough.InvalidState" => Conflict(error),
        "ScenePlaythrough.NotInProgress" => Conflict(error),
        "ScenePlaythrough.CharacterNotFound" => NotFound(error),
        "ScenePlaythrough.ParticipantNotFound" => NotFound(error),
        "ScenePlaythrough.CharacterAlreadyParticipating" => Conflict(error),
        "ScenePlaythrough.InvalidCharacterType" => BadRequest(error),
        "ScenePlaythrough.InvalidStats" => BadRequest(error),
        "ScenePlaythrough.InvalidMovementRoll" => BadRequest(error),
        "ScenePlaythrough.NotCurrentTurn" => Conflict(error),
        "Playthrough.Completed" => Conflict(error),
        _ => BadRequest(error)
    };
}
