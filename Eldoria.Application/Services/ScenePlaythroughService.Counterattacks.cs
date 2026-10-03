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
        AdvanceTurn(scene, turnOwner);
    }

    private static bool CanContinueAttacking(ScenePTParticipant participant) =>
        participant.IsActive &&
        (participant.JourneyPlaythroughCharacter?.CurrentHp ??
            participant.ScenePlaythroughCharacter?.CurrentHp ?? 0) > 0 &&
        participant.JourneyPlaythroughCharacter?.IsDown != true &&
        participant.ScenePlaythroughCharacter?.IsDead != true;

    private static void FinishAttackSequence(ScenePT scene, ScenePTParticipant attacker)
    {
        attacker.AttacksRemaining = 0;
        var defender = scene.SceneParticipants.SingleOrDefault(p => p.Id == attacker.LockedAttackTargetId);
        if (defender is null || !CanContinueAttacking(attacker) || !CanContinueAttacking(defender) ||
            !IsValidAttackTarget(attacker, defender))
        {
            AdvanceTurn(scene, attacker);
            return;
        }

        scene.CounterattackerId = defender.Id;
        scene.CounterattackTargetId = attacker.Id;
        scene.CounterattackToken = Guid.NewGuid();
        var defenderName = defender.JourneyPlaythroughCharacter?.PlaythroughCharacter.Name
            ?? defender.ScenePlaythroughCharacter?.PlaythroughCharacter.Name;
        var attackerName = attacker.JourneyPlaythroughCharacter?.PlaythroughCharacter.Name
            ?? attacker.ScenePlaythroughCharacter?.PlaythroughCharacter.Name;
        AddEvent(scene, $"{defenderName} may counterattack {attackerName}");
    }

    // GM changes/removals can invalidate a target between attacks.
    private static void ForfeitUnavailableAttackSequence(ScenePT scene)
    {
        var attacker = scene.SceneParticipants.SingleOrDefault(p => p.Id == scene.CurrentParticipantId);
        if (attacker?.LockedAttackTargetId is not int targetId || scene.CounterattackToken is not null)
            return;
        var target = scene.SceneParticipants.SingleOrDefault(p => p.Id == targetId);
        if (target is null || !CanContinueAttacking(attacker) || !CanContinueAttacking(target) ||
            !IsValidAttackTarget(attacker, target))
            AdvanceTurn(scene, attacker);
    }

    private static void ClearCounterattack(ScenePT scene)
    {
        scene.CounterattackerId = null;
        scene.CounterattackTargetId = null;
        scene.CounterattackToken = null;
    }
}
