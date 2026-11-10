using Eldoria.Application.Common;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Services;

public sealed partial class ScenePlaythroughService
{
    private async Task<ScenePT?> GetSceneForTurnAsync(int userId, int playthroughId, int sceneId, CancellationToken ct)
    {
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(userId, playthroughId, sceneId, ct);
        if (scene?.Status == ScenePlaythroughStatus.InProgress && scene.CurrentParticipantId is not null)
            playthroughRepository.CaptureTurnCheckpoint(scene);
        return scene;
    }

    public async Task<Result> BeginTurnAsync(int userId, int playthroughId, int sceneId, int participantId, CancellationToken ct)
    {
        await using var transaction = await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(userId, playthroughId, sceneId, ct);
        var error = ValidateManageableScene(scene);
        if (error is not null) return Result.Fail(error);
        if (scene!.CurrentParticipantId != participantId)
            return Result.Fail(new Error("ScenePlaythrough.NotCurrentTurn", "This participant does not have the current turn."));
        playthroughRepository.CaptureTurnCheckpoint(scene);
        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
    }

    public async Task<Result> UndoTurnAsync(int userId, int playthroughId, int sceneId, Guid token, CancellationToken ct)
    {
        await using var transaction = await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(userId, playthroughId, sceneId, ct);
        var error = ValidateManageableScene(scene, allowCounterattack: true);
        if (error is not null) return Result.Fail(error);
        if (scene!.UndoToken != token || scene.TurnCheckpointJson is null ||
            scene.UndoRevision != scene.Playthrough.GameplayRevision)
            return Result.Fail(new Error("ScenePlaythrough.UndoUnavailable", "This turn can no longer be undone. Reload the scene."));
        await playthroughRepository.RestoreTurnCheckpointAsync(scene, ct);
        AddEvent(scene, "The game master undid the most recent turn. Its actions and results have been reverted; earlier log entries are retained for history.");
        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
    }
}
