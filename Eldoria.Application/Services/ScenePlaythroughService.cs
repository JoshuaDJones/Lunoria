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

    public async Task<Result> ActivateJourneyCharacterAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int journeyPlaythroughCharacterId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result.Fail(stateError);

        var character = scene!.Playthrough.JourneyCharacters.SingleOrDefault(
            item => item.Id == journeyPlaythroughCharacterId);
        if (character is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.CharacterNotFound",
                "The journey character was not found."));
        }

        if (scene.SceneParticipants.Any(participant =>
            participant.JourneyPlaythroughCharacterId == character.Id))
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.CharacterAlreadyParticipating",
                "The journey character is already participating in this scene."));
        }

        character.IsActive = true;
        var participant = new ScenePTParticipant
        {
            IsActive = true,
            SortOrderWithinType = NextSortOrder(scene, ParticipantType.Player),
            ParticipantType = ParticipantType.Player,
            JourneyPlaythroughCharacter = character
        };
        scene.SceneParticipants.Add(participant);
        scene.CurrentParticipant ??= participant;
        AddEvent(scene, $"Activated {character.PlaythroughCharacter.Name}");

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
    }

    public async Task<Result> AddPlaythroughCharacterAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int playthroughCharacterId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result.Fail(stateError);

        var character = scene!.Playthrough.Characters.SingleOrDefault(
            item => item.Id == playthroughCharacterId);
        if (character is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.CharacterNotFound",
                "The playthrough character was not found."));
        }

        var participantType = GetSceneParticipantType(character.CharacterType);
        if (participantType is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.InvalidCharacterType",
                "Only NPC or enemy characters can be added to a scene."));
        }

        var instance = CreateSceneCharacter(character);
        scene.SceneCharacters.Add(instance);
        scene.SceneParticipants.Add(new ScenePTParticipant
        {
            IsActive = true,
            SortOrderWithinType = NextSortOrder(scene, participantType.Value),
            ParticipantType = participantType.Value,
            ScenePlaythroughCharacter = instance
        });
        AddEvent(scene, $"Added {character.Name} to {scene.Name}");

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
    }

    public async Task<Result> UpdateParticipantStatsAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        SceneParticipantStatsUpdateDto update,
        CancellationToken ct)
    {
        if (update.CurrentHp < 0 || update.MaxHp < 1 ||
            update.CurrentMp < 0 || update.MaxMp < 0 || update.Movement < 0 ||
            update.CurrentHp > update.MaxHp || update.CurrentMp > update.MaxMp)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.InvalidStats",
                "Participant stats are outside the allowed range."));
        }

        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result.Fail(stateError);

        var participant = scene!.SceneParticipants.SingleOrDefault(
            item => item.Id == participantId);
        if (participant is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The scene participant was not found."));
        }

        if (participant.JourneyPlaythroughCharacter is { } journeyCharacter)
        {
            journeyCharacter.CurrentHp = update.CurrentHp;
            journeyCharacter.MaxHp = update.MaxHp;
            journeyCharacter.CurrentMp = update.CurrentMp;
            journeyCharacter.MaxMp = update.MaxMp;
            journeyCharacter.Movement = update.Movement;
            journeyCharacter.MeleeAttackDamage = update.MeleeAttackDamage;
            journeyCharacter.BowAttackDamage = update.BowAttackDamage;
            journeyCharacter.IsDown = update.CurrentHp == 0;
            AddEvent(scene, $"Adjusted stats for {journeyCharacter.PlaythroughCharacter.Name}");
        }
        else if (participant.ScenePlaythroughCharacter is { } sceneCharacter)
        {
            sceneCharacter.CurrentHp = update.CurrentHp;
            sceneCharacter.MaxHp = update.MaxHp;
            sceneCharacter.CurrentMp = update.CurrentMp;
            sceneCharacter.MaxMp = update.MaxMp;
            sceneCharacter.Movement = update.Movement;
            sceneCharacter.MeleeAttackDamage = update.MeleeAttackDamage;
            sceneCharacter.BowAttackDamage = update.BowAttackDamage;
            sceneCharacter.IsDead = update.CurrentHp == 0;
            AddEvent(scene, $"Adjusted stats for {sceneCharacter.PlaythroughCharacter.Name}");
        }

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
    }

    public async Task<Result<SceneMovementResultDto>> RecordMovementAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int roll,
        CancellationToken ct)
    {
        if (roll is < 1 or > 6)
        {
            return Result<SceneMovementResultDto>.Fail(new Error(
                "ScenePlaythrough.InvalidMovementRoll",
                "The movement roll must be between 1 and 6."));
        }

        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result<SceneMovementResultDto>.Fail(stateError);

        var participant = scene!.SceneParticipants.SingleOrDefault(
            item => item.Id == participantId);
        if (participant is null)
        {
            return Result<SceneMovementResultDto>.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The scene participant was not found."));
        }

        if (scene.CurrentParticipantId != participant.Id || !participant.IsActive)
        {
            return Result<SceneMovementResultDto>.Fail(new Error(
                "ScenePlaythrough.NotCurrentTurn",
                "Movement can only be recorded for the current participant."));
        }

        var journeyCharacter = participant.JourneyPlaythroughCharacter;
        var sceneCharacter = participant.ScenePlaythroughCharacter;
        var characterName = journeyCharacter?.PlaythroughCharacter.Name
            ?? sceneCharacter?.PlaythroughCharacter.Name
            ?? "Unknown character";
        var baseMovement = journeyCharacter?.Movement
            ?? sceneCharacter?.Movement
            ?? 0;
        var movement = baseMovement + roll;

        AddEvent(scene, $"{characterName} moved {movement} spaces");
        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result<SceneMovementResultDto>.Ok(new SceneMovementResultDto
        {
            Movement = movement
        });
    }

    public async Task<Result> ForfeitActionAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result.Fail(stateError);

        var participant = scene!.SceneParticipants.SingleOrDefault(
            item => item.Id == participantId);
        if (participant is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The scene participant was not found."));
        }

        if (scene.CurrentParticipantId != participant.Id)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.NotCurrentTurn",
                "Only the current participant can forfeit an action."));
        }

        var characterName = participant.JourneyPlaythroughCharacter
                ?.PlaythroughCharacter.Name
            ?? participant.ScenePlaythroughCharacter?.PlaythroughCharacter.Name
            ?? "Unknown character";
        AddEvent(scene, $"{characterName} forfeited their action");
        AdvanceTurn(scene, participant);

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

    private static ScenePTCharacter CreateSceneCharacter(
        PlaythroughCharacter character)
    {
        return new ScenePTCharacter
        {
            SourceSceneCharacterId = null,
            InitialMeleeAttackDamage = character.BaseMeleeAttackDamage,
            InitialBowAttackDamage = character.BaseBowAttackDamage,
            InitialMovement = character.BaseMovement,
            InitialMaxConsumableInventory = character.BaseMaxConsumableInventory,
            InitialMaxEquippableInventory = character.BaseMaxEquippableInventory,
            InitialMaxHp = character.BaseMaxHp,
            InitialMaxMp = character.BaseMaxMp,
            IsInitiallyActive = true,
            MeleeAttackDamage = character.BaseMeleeAttackDamage,
            BowAttackDamage = character.BaseBowAttackDamage,
            Movement = character.BaseMovement,
            MaxConsumableInventory = character.BaseMaxConsumableInventory,
            MaxEquippableInventory = character.BaseMaxEquippableInventory,
            CurrentHp = character.BaseMaxHp,
            CurrentMp = character.BaseMaxMp,
            MaxHp = character.BaseMaxHp,
            MaxMp = character.BaseMaxMp,
            IsActive = true,
            IsDead = false,
            IsInAlternateForm = false,
            PlaythroughCharacterId = character.Id,
            AlternateFormId = character.BaseAlternateFormId,
            Spells = [.. character.Spells.Select(spell =>
                new ScenePTCharacterSpell
                {
                    SourceSceneCharacterSpellId = null,
                    PlaythroughSpellId = spell.PlaythroughSpellId
                })]
        };
    }

    private static Error? ValidateManageableScene(ScenePT? scene)
    {
        if (scene is null)
            return new Error("ScenePlaythrough.NotFound", "Scene playthrough was not found.");
        if (scene.Playthrough.CompletedAt is not null)
            return new Error("Playthrough.Completed", "The playthrough is completed.");
        return scene.Status == ScenePlaythroughStatus.InProgress
            ? null
            : new Error(
                "ScenePlaythrough.NotInProgress",
                "Scene options are only available while the scene is in progress.");
    }

    private static ParticipantType? GetSceneParticipantType(CharacterType type) =>
        type switch
        {
            CharacterType.NPC => ParticipantType.NPC,
            CharacterType.Enemy => ParticipantType.Enemy,
            _ => null
        };

    private static int NextSortOrder(ScenePT scene, ParticipantType type) =>
        scene.SceneParticipants
            .Where(participant => participant.ParticipantType == type)
            .Select(participant => participant.SortOrderWithinType ?? -1)
            .DefaultIfEmpty(-1)
            .Max() + 1;

    private static void AddEvent(ScenePT scene, string message)
    {
        scene.Playthrough.EventLogs.Add(new PlaythroughEventLog
        {
            Message = message,
            EventTime = DateTime.UtcNow
        });
    }

    private static void AdvanceTurn(
        ScenePT scene,
        ScenePTParticipant currentParticipant)
    {
        var participants = scene.SceneParticipants
            .Where(participant => participant.IsActive)
            .OrderBy(participant => participant.ParticipantType)
            .ThenBy(participant => participant.SortOrderWithinType)
            .ThenBy(participant => participant.Id)
            .ToList();
        var currentIndex = participants.FindIndex(
            participant => participant.Id == currentParticipant.Id);

        if (participants.Count == 0 || currentIndex < 0)
        {
            scene.CurrentParticipant = null;
            return;
        }

        var nextIndex = (currentIndex + 1) % participants.Count;
        if (nextIndex == 0)
            scene.RoundNumber++;

        scene.CurrentParticipant = participants[nextIndex];
    }
}
