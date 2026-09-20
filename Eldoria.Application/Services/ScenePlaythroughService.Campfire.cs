using Eldoria.Application.Common;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Services;

public sealed partial class ScenePlaythroughService
{
    public async Task<Result> CampfireAsync(int userId, int playthroughId, int sceneId,
        int participantId, string resource, CancellationToken ct)
    {
        if (resource is not ("hp" or "mp"))
            return Result.Fail(new Error("ScenePlaythrough.InvalidCampfireResource", "Choose HP or MP to replenish."));

        await using var transaction = await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(userId, playthroughId, sceneId, ct);
        var error = ValidateManageableScene(scene);
        if (error is not null) return Result.Fail(error);

        var participant = scene!.SceneParticipants.SingleOrDefault(p => p.Id == participantId);
        if (participant is null)
            return Result.Fail(new Error("ScenePlaythrough.ParticipantNotFound", "The scene participant was not found."));
        var character = participant.JourneyPlaythroughCharacter;
        if (participant.ParticipantType != ParticipantType.Player || character is null)
            return Result.Fail(new Error("ScenePlaythrough.CampfireUnavailable", "Only journey players can use a campfire."));
        if (scene.CurrentParticipantId != participant.Id || !participant.IsActive || character.IsDown || character.CurrentHp <= 0)
            return Result.Fail(new Error("ScenePlaythrough.NotCurrentTurn", "Only the active, standing player whose turn it is can use a campfire."));
        if (participant.AttacksRemaining <= 0)
            return Result.Fail(new Error("ScenePlaythrough.NoAttacksRemaining", "This player has no action remaining."));

        if (ValidateGeneralAction(participant) is { } actionError)
            return Result.Fail(actionError);

        // Proximity is checked by the players on the physical board.
        // HP/MP belong to the journey character even while transformed.
        var equipment = ScenePlaythroughEquipmentEffects.For(participant);
        if (resource == "hp")
            character.CurrentHp = ScenePlaythroughEquipmentEffects.Apply(character.MaxHp, equipment.MaxHpModifier, minimum: 1);
        else
            character.CurrentMp = ScenePlaythroughEquipmentEffects.Apply(character.MaxMp, equipment.MaxMpModifier);

        AddEvent(scene, $"{character.PlaythroughCharacter.Name} replenished {resource.ToUpperInvariant()} at the campfire");
        AdvanceTurn(scene, participant);
        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
    }
}
