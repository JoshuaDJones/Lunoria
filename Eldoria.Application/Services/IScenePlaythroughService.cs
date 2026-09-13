using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Services;

public interface IScenePlaythroughService
{
    Task<Result<SceneStartResultDto>> ContinueStartAsync(int userId, int playthroughId, int sceneId,
        SceneInventoryResolutionInput? resolution, CancellationToken ct);
    Task<Result<SceneStartResultDto>> GetStartInventoryAsync(int userId, int playthroughId, int sceneId,
        CancellationToken ct);
    Task<Result> StartAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<Result> EndAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<Result<ScenePlaythroughDetailsDto>> GetAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<Result> AddSceneCharacterInstanceAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int scenePlaythroughCharacterId,
        CancellationToken ct);

    Task<Result> ActivateJourneyCharacterAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int journeyPlaythroughCharacterId,
        CancellationToken ct);

    Task<Result> AddPlaythroughCharacterAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int playthroughCharacterId,
        CancellationToken ct);

    Task<Result> RemoveSceneParticipantAsync(
        int userId, int playthroughId, int sceneId, int participantId, CancellationToken ct);

    Task<Result> AddChestAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CreateScenePlaythroughChestDto input,
        CancellationToken ct);

    Task<Result> UpdateParticipantStatsAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        SceneParticipantStatsUpdateDto update,
        CancellationToken ct);

    Task<Result<SceneMovementResultDto>> RecordMovementAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int roll,
        CancellationToken ct);

    Task<Result<SceneAttackResultDto>> AttackAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int? targetParticipantId,
        SceneAttackType attackType,
        int roll,
        int? playthroughSpellId,
        CancellationToken ct,
        bool isCounterattack = false,
        Guid? counterattackToken = null);

    Task<Result> PassCounterattackAsync(int userId, int playthroughId, int sceneId, Guid token, CancellationToken ct);

    Task<Result<SceneOpenChestResultDto>> OpenChestAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int chestId,
        int roll,
        CancellationToken ct);

    Task<Result<SceneUseConsumableResultDto>> UseConsumableAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int inventoryItemId,
        CancellationToken ct);

    Task<Result> TradeItemAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int targetParticipantId,
        int inventoryItemId,
        bool isEquippable,
        CancellationToken ct);

    Task<Result> TransformAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        CancellationToken ct);

    Task<Result> ForfeitActionAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        CancellationToken ct);
}
