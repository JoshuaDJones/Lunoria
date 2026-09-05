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

    [HttpPost("end")]
    public async Task<IActionResult> End(
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.EndAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            ct);

        return await CompleteMutationAsync(result, playthroughId, "SceneEnded", ct);
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

    [HttpPost("chests")]
    public async Task<IActionResult> AddChest(
        int playthroughId,
        int sceneId,
        [FromBody] AddScenePlaythroughChestRequest request,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.AddChestAsync(
            User.GetUserId(), playthroughId, sceneId, request.ToDto(), ct);
        return await CompleteMutationAsync(
            result, playthroughId, "SceneChestAdded", ct);
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

    [HttpPost("participants/{participantId:int}/attack")]
    public async Task<ActionResult<SceneAttackResultDto>> Attack(
        int playthroughId,
        int sceneId,
        int participantId,
        [FromBody] ResolveSceneAttackRequest request,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.AttackAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            participantId,
            request.TargetParticipantId,
            request.AttackType,
            request.Roll,
            request.PlaythroughSpellId,
            ct);

        if (result.Success)
        {
            await realtimeNotifier.NotifyUpdatedAsync(
                playthroughId, "AttackResolved", ct);
            return Ok(result.Value);
        }

        return ToError(result.Error);
    }

    [HttpPost("participants/{participantId:int}/chests/{chestId:int}/open")]
    public async Task<ActionResult<SceneOpenChestResultDto>> OpenChest(
        int playthroughId,
        int sceneId,
        int participantId,
        int chestId,
        [FromBody] OpenSceneChestRequest request,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.OpenChestAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            participantId,
            chestId,
            request.Roll,
            ct);

        if (result.Success)
        {
            var chestResult = result.Value!;
            await realtimeNotifier.NotifyUpdatedAsync(
                playthroughId,
                chestResult.Awarded ? "ChestOpened" : "ActionForfeited",
                ct);
            return Ok(chestResult);
        }

        return ToError(result.Error);
    }

    [HttpPost("participants/{participantId:int}/consumables/{inventoryItemId:int}/use")]
    public async Task<ActionResult<SceneUseConsumableResultDto>> UseConsumable(
        int playthroughId,
        int sceneId,
        int participantId,
        int inventoryItemId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.UseConsumableAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            participantId,
            inventoryItemId,
            ct);

        if (result.Success)
        {
            await realtimeNotifier.NotifyUpdatedAsync(
                playthroughId, "ConsumableUsed", ct);
            return Ok(result.Value);
        }

        return ToError(result.Error);
    }

    [HttpPost("participants/{participantId:int}/trade")]
    public async Task<IActionResult> TradeItem(
        int playthroughId,
        int sceneId,
        int participantId,
        [FromBody] TradeSceneItemRequest request,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.TradeItemAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            participantId,
            request.TargetParticipantId,
            request.InventoryItemId,
            request.IsEquippable,
            ct);

        return await CompleteMutationAsync(result, playthroughId, "ItemTraded", ct);
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

    private ActionResult ToError(Error error) => error.Code switch
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
        "ScenePlaythrough.InvalidAttackRoll" => BadRequest(error),
        "ScenePlaythrough.InvalidAttackType" => BadRequest(error),
        "ScenePlaythrough.InvalidAttackTarget" => BadRequest(error),
        "ScenePlaythrough.AttackUnavailable" => Conflict(error),
        "ScenePlaythrough.SpellNotFound" => NotFound(error),
        "ScenePlaythrough.InsufficientMp" => Conflict(error),
        "ScenePlaythrough.InvalidChestRoll" => BadRequest(error),
        "ScenePlaythrough.ChestNotFound" => NotFound(error),
        "ScenePlaythrough.ChestAlreadyOpened" => Conflict(error),
        "ScenePlaythrough.ChestUnavailable" => Conflict(error),
        "ScenePlaythrough.ChestLootNotConfigured" => Conflict(error),
        "ScenePlaythrough.ChestItemNotFound" => NotFound(error),
        "ScenePlaythrough.ConsumableNotFound" => NotFound(error),
        "ScenePlaythrough.UseConsumableUnavailable" => Conflict(error),
        "ScenePlaythrough.InventoryFull" => Conflict(error),
        "ScenePlaythrough.TradeUnavailable" => Conflict(error),
        "ScenePlaythrough.TradeItemNotFound" => NotFound(error),
        "ScenePlaythrough.TradeInventoryFull" => Conflict(error),
        "ScenePlaythrough.NotCurrentTurn" => Conflict(error),
        "ScenePlaythrough.EventExecutionFailed" => Conflict(error),
        "Playthrough.Completed" => Conflict(error),
        _ => BadRequest(error)
    };
}
