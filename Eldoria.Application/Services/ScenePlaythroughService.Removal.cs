using Eldoria.Application.Common;

namespace Eldoria.Application.Services;

public sealed partial class ScenePlaythroughService
{
    public async Task<Result> RemoveSceneParticipantAsync(
        int userId, int playthroughId, int sceneId, int participantId, CancellationToken ct)
    {
        await using var transaction = await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var error = ValidateManageableScene(scene);
        if (error is not null) return Result.Fail(error);

        var participant = scene!.SceneParticipants.SingleOrDefault(p => p.Id == participantId);
        if (participant is null)
            return Result.Fail(new Error("ScenePlaythrough.ParticipantNotFound", "The scene participant was not found."));
        if (participant.JourneyPlaythroughCharacterId is not null || participant.ScenePlaythroughCharacter is null)
            return Result.Fail(new Error("ScenePlaythrough.InvalidCharacterType", "Journey characters cannot be removed with this action."));

        var character = participant.ScenePlaythroughCharacter;
        participant.IsActive = false;
        character.IsActive = false;
        scene.SceneParticipants.Remove(participant);
        if (scene.CurrentParticipantId == participant.Id)
            AdvanceTurn(scene, participant);

        AddEvent(scene, $"Removed {character.PlaythroughCharacter.Name} from {scene.Name}");
        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
    }
}
