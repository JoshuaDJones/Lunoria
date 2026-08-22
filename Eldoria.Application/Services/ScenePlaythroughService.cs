using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services;

public sealed class ScenePlaythroughService(
    IPlaythroughRepository playthroughRepository) : IScenePlaythroughService
{
    public async Task<Result> StartAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);

        var scene = await playthroughRepository.GetSceneForStartAsync(
            userId,
            playthroughId,
            sceneId,
            ct);

        if (scene is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.NotFound",
                "Scene playthrough was not found."));
        }

        if (scene.Playthrough.CompletedAt is not null)
        {
            return Result.Fail(new Error(
                "Playthrough.Completed",
                "A scene cannot be started in a completed playthrough."));
        }

        if (scene.Status != ScenePlaythroughStatus.NotStarted)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.AlreadyStarted",
                "The scene has already been started."));
        }

        if (scene.SceneParticipants.Count != 0)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.InvalidState",
                "The unstarted scene already has participants."));
        }

        var journeyParticipants = scene.Playthrough.JourneyCharacters
            .Where(character => character.IsActive)
            .OrderBy(character => character.SourceJourneyCharacterId)
            .Select((character, index) => new ScenePTParticipant
            {
                IsActive = true,
                SortOrderWithinType = index,
                ParticipantType = ParticipantType.Player,
                JourneyPlaythroughCharacter = character
            })
            .ToList();

        var npcParticipants = CreateSceneCharacterParticipants(
            scene,
            CharacterType.NPC,
            ParticipantType.NPC);
        var enemyParticipants = CreateSceneCharacterParticipants(
            scene,
            CharacterType.Enemy,
            ParticipantType.Enemy);
        var participants = journeyParticipants
            .Concat(npcParticipants)
            .Concat(enemyParticipants)
            .ToList();

        foreach (var participant in participants)
            scene.SceneParticipants.Add(participant);

        var startedAt = DateTime.UtcNow;
        scene.Status = ScenePlaythroughStatus.InProgress;
        scene.StartedAt = startedAt;
        scene.RoundNumber = 1;
        scene.CurrentParticipant = journeyParticipants.FirstOrDefault();
        scene.Playthrough.EventLogs.Add(new PlaythroughEventLog
        {
            Message = $"Scene Started: {scene.Name}",
            EventTime = startedAt
        });

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Ok();
    }

    public async Task<Result<ScenePlaythroughDetailsDto>> GetAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        var scene = await playthroughRepository.GetSceneDetailsAsync(
            userId,
            playthroughId,
            sceneId,
            ct);

        return scene is null
            ? Result<ScenePlaythroughDetailsDto>.Fail(new Error(
                "ScenePlaythrough.NotFound",
                "Scene playthrough was not found."))
            : Result<ScenePlaythroughDetailsDto>.Ok(scene.ToDetailsDto());
    }

    private static List<ScenePTParticipant> CreateSceneCharacterParticipants(
        ScenePT scene,
        CharacterType characterType,
        ParticipantType participantType)
    {
        return scene.SceneCharacters
            .Where(character =>
                character.IsActive &&
                character.PlaythroughCharacter.CharacterType == characterType)
            .OrderBy(character => character.SourceSceneCharacterId)
            .Select((character, index) => new ScenePTParticipant
            {
                IsActive = true,
                SortOrderWithinType = index,
                ParticipantType = participantType,
                ScenePlaythroughCharacter = character
            })
            .ToList();
    }
}
