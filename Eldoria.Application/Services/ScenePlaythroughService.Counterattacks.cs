using Eldoria.Application.Common;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Services;

public sealed partial class ScenePlaythroughService
{
    public async Task<Result> PassCounterattackAsync(int userId, int playthroughId, int sceneId, Guid token, CancellationToken ct)
    {
        await using var transaction = await playthroughRepository.BeginSceneStartTransactionAsync(ct);

        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(userId, playthroughId, sceneId, ct);
        var error = ValidateManageableScene(scene, allowCounterattack: true);

        if (error is not null) 
            return Result.Fail(error);

        if (scene!.CounterattackToken is null || scene.CounterattackToken != token)
            return Result.Fail(new Error("ScenePlaythrough.InvalidCounterattack", "This counterattack is no longer available. Reload the scene."));

        var turnOwner = scene.SceneParticipants.SingleOrDefault(p => p.Id == scene.CounterattackTargetId);

        if (turnOwner is null) 
            return Result.Fail(new Error("ScenePlaythrough.InvalidState", "The original attacker was not found."));

        var defender = scene.SceneParticipants.SingleOrDefault(p => p.Id == scene.CounterattackerId);
        var name = defender?.JourneyPlaythroughCharacter?.PlaythroughCharacter.Name ?? defender?.ScenePlaythroughCharacter?.PlaythroughCharacter.Name ?? "Defender";

        AddEvent(scene, $"{name} passed their counterattack");
        ClearCounterattack(scene);
        ResumeTurnAfterCounterattack(scene, turnOwner);

        await playthroughRepository.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);
        return Result.Ok();
    }

    private static void ResumeTurnAfterCounterattack(ScenePT scene, ScenePTParticipant turnOwner)
    {
        var currentHp = turnOwner.JourneyPlaythroughCharacter?.CurrentHp
            ?? turnOwner.ScenePlaythroughCharacter?.CurrentHp ?? 0;
        if (!turnOwner.IsActive || currentHp <= 0 ||
            turnOwner.JourneyPlaythroughCharacter?.IsDown == true ||
            turnOwner.ScenePlaythroughCharacter?.IsDead == true ||
            turnOwner.AttacksRemaining <= 0)
        {
            AdvanceTurn(scene, turnOwner);
        }
        // Otherwise preserve the current participant and their remaining attacks.
    }

    private static void ClearCounterattack(ScenePT scene)
    {
        scene.CounterattackerId = null;
        scene.CounterattackTargetId = null;
        scene.CounterattackToken = null;
    }
}
