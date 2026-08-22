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

    public async Task<Result> AddSceneCharacterInstanceAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int scenePlaythroughCharacterId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);

        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
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
                "A character cannot be added to a completed playthrough."));
        }

        if (scene.Status != ScenePlaythroughStatus.InProgress)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.NotInProgress",
                "Characters can only be added while the scene is in progress."));
        }

        var sourceCharacter = scene.SceneCharacters.SingleOrDefault(
            character => character.Id == scenePlaythroughCharacterId);

        if (sourceCharacter is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.CharacterNotFound",
                "The scene character instance was not found."));
        }

        var participantType = sourceCharacter.PlaythroughCharacter.CharacterType switch
        {
            CharacterType.NPC => ParticipantType.NPC,
            CharacterType.Enemy => ParticipantType.Enemy,
            _ => (ParticipantType?)null
        };

        if (participantType is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.InvalidCharacterType",
                "Only NPC or enemy scene characters can be added as new instances."));
        }

        var characterInstance = CloneSceneCharacter(sourceCharacter);
        var sortOrder = scene.SceneParticipants
            .Where(participant => participant.ParticipantType == participantType.Value)
            .Select(participant => participant.SortOrderWithinType ?? -1)
            .DefaultIfEmpty(-1)
            .Max() + 1;

        scene.SceneCharacters.Add(characterInstance);
        scene.SceneParticipants.Add(new ScenePTParticipant
        {
            IsActive = true,
            SortOrderWithinType = sortOrder,
            ParticipantType = participantType.Value,
            ScenePlaythroughCharacter = characterInstance
        });
        scene.Playthrough.EventLogs.Add(new PlaythroughEventLog
        {
            Message = $"Added {sourceCharacter.PlaythroughCharacter.Name} to {scene.Name}",
            EventTime = DateTime.UtcNow
        });

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Ok();
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

    private static ScenePTCharacter CloneSceneCharacter(
        ScenePTCharacter sourceCharacter)
    {
        return new ScenePTCharacter
        {
            SourceSceneCharacterId = sourceCharacter.SourceSceneCharacterId,
            InitialMeleeAttackDamage = sourceCharacter.InitialMeleeAttackDamage,
            InitialBowAttackDamage = sourceCharacter.InitialBowAttackDamage,
            InitialMovement = sourceCharacter.InitialMovement,
            InitialMaxConsumableInventory =
                sourceCharacter.InitialMaxConsumableInventory,
            InitialMaxEquippableInventory =
                sourceCharacter.InitialMaxEquippableInventory,
            InitialMaxHp = sourceCharacter.InitialMaxHp,
            InitialMaxMp = sourceCharacter.InitialMaxMp,
            IsInitiallyActive = sourceCharacter.IsInitiallyActive,
            MeleeAttackDamage = sourceCharacter.InitialMeleeAttackDamage,
            BowAttackDamage = sourceCharacter.InitialBowAttackDamage,
            Movement = sourceCharacter.InitialMovement,
            MaxConsumableInventory = sourceCharacter.InitialMaxConsumableInventory,
            MaxEquippableInventory = sourceCharacter.InitialMaxEquippableInventory,
            CurrentHp = sourceCharacter.InitialMaxHp,
            CurrentMp = sourceCharacter.InitialMaxMp,
            MaxHp = sourceCharacter.InitialMaxHp,
            MaxMp = sourceCharacter.InitialMaxMp,
            IsActive = true,
            IsDead = false,
            IsInAlternateForm = false,
            PlaythroughCharacterId = sourceCharacter.PlaythroughCharacterId,
            AlternateFormId = sourceCharacter.AlternateFormId,
            Spells = [.. sourceCharacter.Spells.Select(spell =>
                new ScenePTCharacterSpell
                {
                    SourceSceneCharacterSpellId = spell.SourceSceneCharacterSpellId,
                    PlaythroughSpellId = spell.PlaythroughSpellId
                })],
            ConsumableItems = [.. sourceCharacter.ConsumableItems.Select(item =>
                new ScenePTCharacterConsumableItem
                {
                    IsUsed = false,
                    PlaythroughConsumableItemId = item.PlaythroughConsumableItemId
                })],
            EquippableItems = [.. sourceCharacter.EquippableItems.Select(item =>
                new ScenePTCharacterEquippableItem
                {
                    IsEquipped = item.IsEquipped,
                    PlaythroughEquippableItemId = item.PlaythroughEquippableItemId
                })]
        };
    }
}
