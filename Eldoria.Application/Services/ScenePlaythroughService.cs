using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using System.Security.Cryptography;

namespace Eldoria.Application.Services;

public sealed partial class ScenePlaythroughService(
    IPlaythroughRepository playthroughRepository) : IScenePlaythroughService
{
    private const int DownedScheduledTurns = 5;

    public async Task<Result> StartAsync(
        int userId, int playthroughId, int sceneId, CancellationToken ct)
    {
        var result = await ContinueStartAsync(userId, playthroughId, sceneId, null, ct);
        return result.Success ? Result.Ok() : Result.Fail(result.Error);
    }

    public async Task<Result> EndAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);

        var scene = await playthroughRepository.GetSceneForEndAsync(
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
                "A scene cannot be ended in a completed playthrough."));
        }

        if (scene.Status != ScenePlaythroughStatus.InProgress)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.NotInProgress",
                "Only an in-progress scene can be ended."));
        }

        if (scene.CounterattackToken is not null)
            return Result.Fail(new Error("ScenePlaythrough.CounterattackPending", "Resolve or pass the counterattack before ending the scene."));

        // Transformation lasts for this scene only. Keep inventories, stats,
        // and alternate-form assignments, including for inactive characters.
        foreach (var character in scene.Playthrough.JourneyCharacters)
            character.IsInAlternateForm = false;

        foreach (var character in scene.SceneCharacters)
            character.IsInAlternateForm = false;

        var endedAt = DateTime.UtcNow;
        scene.Status = ScenePlaythroughStatus.Completed;
        scene.EndedAt = endedAt;
        scene.CurrentParticipantId = null;
        scene.CurrentParticipant = null;
        scene.Playthrough.EventLogs.Add(new PlaythroughEventLog
        {
            Message = $"Scene Ended: {scene.Name}",
            EventTime = endedAt
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
            SortOrderWithinType = character.SortOrder,
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

    public async Task<Result> AddChestAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CreateScenePlaythroughChestDto input,
        CancellationToken ct)
    {
        var inputError = ValidateChestInput(input);
        if (inputError is not null)
            return Result.Fail(inputError);

        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result.Fail(stateError);

        var consumables = scene!.Playthrough.ConsumableItems
            .ToDictionary(item => item.Id);
        var equippables = scene.Playthrough.EquippableItems
            .ToDictionary(item => item.Id);
        var lootEntries = new List<ScenePTChestLootEntry>(input.LootEntries.Count);

        foreach (var entry in input.LootEntries)
        {
            PlaythroughConsumableItem? consumable = null;
            PlaythroughEquippableItem? equippable = null;

            if (entry.PlaythroughConsumableItemId is int consumableId &&
                !consumables.TryGetValue(consumableId, out consumable))
            {
                return Result.Fail(new Error(
                    "ScenePlaythrough.ChestItemNotFound",
                    "The selected consumable item is not available in this playthrough."));
            }

            if (entry.PlaythroughEquippableItemId is int equippableId &&
                !equippables.TryGetValue(equippableId, out equippable))
            {
                return Result.Fail(new Error(
                    "ScenePlaythrough.ChestItemNotFound",
                    "The selected equippable item is not available in this playthrough."));
            }

            lootEntries.Add(new ScenePTChestLootEntry
            {
                SourceSceneChestLootEntryId = null,
                RollMinimum = entry.RollMinimum,
                RollMaximum = entry.RollMaximum,
                Quantity = entry.Quantity,
                PlaythroughConsumableItem = consumable,
                PlaythroughEquippableItem = equippable
            });
        }

        var chest = new ScenePTChest
        {
            SourceSceneChestId = null,
            Name = input.Name.Trim(),
            DieSides = input.DieSides,
            Status = ChestStatus.Unopened,
            ChestLootEntries = lootEntries
        };
        scene.SceneChests.Add(chest);
        AddEvent(scene, $"Added chest {chest.Name} to {scene.Name}");

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
            update.CurrentMp < 0 || update.MaxMp < 0 ||
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
            participant.DownedTurnsRemaining = journeyCharacter.IsDown
                ? DownedScheduledTurns
                : null;
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
        var baseMovement = SceneActiveCombatStats.For(participant).Movement;
        var equipmentEffects = ScenePlaythroughEquipmentEffects.For(participant);
        var movement = ScenePlaythroughEquipmentEffects.Apply(
            ScenePlaythroughEquipmentEffects.Apply(
                baseMovement,
                equipmentEffects.MovementModifier, minimum: int.MinValue),
            roll);

        AddEvent(scene, $"{characterName} moved {movement} spaces");
        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result<SceneMovementResultDto>.Ok(new SceneMovementResultDto
        {
            Movement = movement
        });
    }

    public async Task<Result<SceneAttackResultDto>> AttackAsync(
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
        Guid? counterattackToken = null)
    {
        if (roll is < 1 or > 6)
        {
            return Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.InvalidAttackRoll",
                "The attack roll must be between 1 and 6."));
        }

        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene, allowCounterattack: isCounterattack);
        if (stateError is not null)
            return Result<SceneAttackResultDto>.Fail(stateError);
        if (isCounterattack && (scene!.CounterattackToken is null || scene.CounterattackToken != counterattackToken ||
            scene.CounterattackerId != participantId || scene.CounterattackTargetId != targetParticipantId))
            return Result<SceneAttackResultDto>.Fail(new Error("ScenePlaythrough.InvalidCounterattack", "This counterattack is no longer available. Reload the scene."));

        var attacker = scene!.SceneParticipants.SingleOrDefault(
            participant => participant.Id == participantId);
        if (attacker is null)
        {
            return Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The attacking participant was not found."));
        }

        if ((!isCounterattack && scene.CurrentParticipantId != attacker.Id) || !attacker.IsActive)
        {
            return Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.NotCurrentTurn",
                "Only the active current participant can attack."));
        }

        if (!isCounterattack && attacker.AttacksRemaining <= 0)
        {
            return Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.NoAttacksRemaining",
                "This participant has no attacks remaining this turn."));
        }

        if (attacker.JourneyPlaythroughCharacter?.IsDown == true ||
            attacker.ScenePlaythroughCharacter?.IsDead == true)
        {
            return Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.AttackUnavailable",
                "A downed or defeated participant cannot attack."));
        }

        var selectedSpell = attackType == SceneAttackType.Spell
            ? GetSpells(attacker).SingleOrDefault(spell => spell.Id == playthroughSpellId)
            : null;
        var isUtility = selectedSpell is not null && selectedSpell.DamageEffect.GetValueOrDefault() == 0
            && selectedSpell.HealthEffect.GetValueOrDefault() == 0 && selectedSpell.MagicEffect.GetValueOrDefault() == 0;
        if (isCounterattack && attackType == SceneAttackType.Spell &&
            (selectedSpell?.DamageEffect is null || isUtility ||
             (selectedSpell.DamageEffect <= 0 && (selectedSpell.HealthEffect > 0 || selectedSpell.MagicEffect > 0))))
            return Result<SceneAttackResultDto>.Fail(new Error("ScenePlaythrough.InvalidCounterattack", "Only damage spells can be used for a counterattack."));
        if (isUtility)
        {
            var journeyCaster = attacker.JourneyPlaythroughCharacter;
            var sceneCaster = attacker.ScenePlaythroughCharacter;
            var currentMp = journeyCaster?.CurrentMp ?? sceneCaster!.CurrentMp;
            if (currentMp < selectedSpell!.MpCost)
                return Result<SceneAttackResultDto>.Fail(new Error("ScenePlaythrough.InsufficientMp", "The participant does not have enough MP to cast this spell."));
            if (journeyCaster is not null) journeyCaster.CurrentMp -= selectedSpell.MpCost;
            else sceneCaster!.CurrentMp -= selectedSpell.MpCost;
            var casterName = journeyCaster?.PlaythroughCharacter.Name ?? sceneCaster!.PlaythroughCharacter.Name;
            AddEvent(scene, $"{casterName} cast {selectedSpell.Name}, spending {selectedSpell.MpCost} MP");
            attacker.AttacksRemaining--;
            if (attacker.AttacksRemaining == 0) AdvanceTurn(scene, attacker);
            await playthroughRepository.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return Result<SceneAttackResultDto>.Ok(new SceneAttackResultDto { IsUtility = true });
        }

        var target = scene.SceneParticipants.SingleOrDefault(
            participant => participant.Id == targetParticipantId);
        if (target is null)
        {
            return Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The target participant was not found."));
        }

        var isSupport = selectedSpell is not null && selectedSpell.DamageEffect.GetValueOrDefault() <= 0
            && (selectedSpell.HealthEffect > 0 || selectedSpell.MagicEffect > 0);
        var validSupportTarget = target.IsActive && target.ScenePlaythroughCharacter?.IsDead != true
            && (attacker.ParticipantType == ParticipantType.Enemy
                ? target.ParticipantType == ParticipantType.Enemy
                : target.ParticipantType is ParticipantType.Player or ParticipantType.NPC);
        if (isSupport ? !validSupportTarget : !IsValidAttackTarget(attacker, target))
        {
            return Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.InvalidAttackTarget",
                "The selected participant cannot be targeted by this attacker."));
        }

        var attackerJourneyCharacter = attacker.JourneyPlaythroughCharacter;
        var attackerSceneCharacter = attacker.ScenePlaythroughCharacter;
        var attackerName = attackerJourneyCharacter?.PlaythroughCharacter.Name
            ?? attackerSceneCharacter?.PlaythroughCharacter.Name
            ?? "Unknown character";
        var targetJourneyCharacter = target.JourneyPlaythroughCharacter;
        var targetSceneCharacter = target.ScenePlaythroughCharacter;
        var targetName = targetJourneyCharacter?.PlaythroughCharacter.Name
            ?? targetSceneCharacter?.PlaythroughCharacter.Name
            ?? "Unknown character";
        var attackerEquipment = ScenePlaythroughEquipmentEffects.For(attacker);
        var targetEquipment = ScenePlaythroughEquipmentEffects.For(target);

        int baseDamage;
        string attackLabel;

        switch (attackType)
        {
            case SceneAttackType.Melee:
                if (playthroughSpellId is not null)
                    return InvalidAttackType("A melee attack cannot include a spell.");

                var meleeDamage = SceneActiveCombatStats.For(attacker).Melee;
                if (meleeDamage is null)
                    return AttackUnavailable("This participant has no melee attack.");

                baseDamage = ScenePlaythroughEquipmentEffects.Apply(
                    meleeDamage.Value,
                    attackerEquipment.MeleeAttackDamageModifier, minimum: int.MinValue);
                attackLabel = "a melee attack";
                break;

            case SceneAttackType.Range:
                if (playthroughSpellId is not null)
                    return InvalidAttackType("A range attack cannot include a spell.");

                var rangeDamage = SceneActiveCombatStats.For(attacker).Bow;
                if (rangeDamage is null)
                    return AttackUnavailable("This participant has no range attack.");

                baseDamage = ScenePlaythroughEquipmentEffects.Apply(
                    rangeDamage.Value,
                    attackerEquipment.BowAttackDamageModifier, minimum: int.MinValue);
                attackLabel = "a range attack";
                break;

            case SceneAttackType.Spell:
                if (playthroughSpellId is null)
                    return InvalidAttackType("A spell attack requires a spell.");

                var spell = GetSpells(attacker).SingleOrDefault(
                    candidate => candidate.Id == playthroughSpellId.Value);
                if (spell is null || (!isSupport && spell.DamageEffect is null))
                {
                    return Result<SceneAttackResultDto>.Fail(new Error(
                        "ScenePlaythrough.SpellNotFound",
                        "The selected spell is not available to this participant."));
                }

                var currentMp = attackerJourneyCharacter?.CurrentMp
                    ?? attackerSceneCharacter?.CurrentMp
                    ?? 0;
                if (currentMp < spell.MpCost)
                {
                    return Result<SceneAttackResultDto>.Fail(new Error(
                        "ScenePlaythrough.InsufficientMp",
                        "The participant does not have enough MP to cast this spell."));
                }

                if (attackerJourneyCharacter is not null)
                    attackerJourneyCharacter.CurrentMp -= spell.MpCost;
                else
                    attackerSceneCharacter!.CurrentMp -= spell.MpCost;

                if (isSupport)
                {
                    var oldHp = targetJourneyCharacter?.CurrentHp ?? targetSceneCharacter!.CurrentHp;
                    var oldMp = targetJourneyCharacter?.CurrentMp ?? targetSceneCharacter!.CurrentMp;
                    var maxHp = ScenePlaythroughEquipmentEffects.Apply(targetJourneyCharacter?.MaxHp ?? targetSceneCharacter!.MaxHp, targetEquipment.MaxHpModifier, minimum: 1);
                    var maxMp = ScenePlaythroughEquipmentEffects.Apply(targetJourneyCharacter?.MaxMp ?? targetSceneCharacter!.MaxMp, targetEquipment.MaxMpModifier);
                    var hpRestoration = spell.HealthEffect > 0 ? (long)spell.HealthEffect.Value + roll : 0;
                    var mpRestoration = spell.MagicEffect > 0 ? (long)spell.MagicEffect.Value + roll : 0;
                    var healedHp = (int)Math.Max(oldHp, Math.Min(maxHp, (long)oldHp + hpRestoration));
                    var healedMp = (int)Math.Max(oldMp, Math.Min(maxMp, (long)oldMp + mpRestoration));
                    if (targetJourneyCharacter is not null)
                    {
                        targetJourneyCharacter.CurrentHp = healedHp;
                        targetJourneyCharacter.CurrentMp = healedMp;
                        if (healedHp > 0 && targetJourneyCharacter.IsDown)
                        {
                            targetJourneyCharacter.IsDown = false;
                            target.DownedTurnsRemaining = null;
                        }
                    }
                    else
                    {
                        targetSceneCharacter!.CurrentHp = healedHp;
                        targetSceneCharacter.CurrentMp = healedMp;
                    }
                    AddEvent(scene, $"{attackerName} cast {spell.Name} on {targetName} with a roll of {roll}, restoring {healedHp - oldHp} HP and {healedMp - oldMp} MP");
                    attacker.AttacksRemaining--;
                    if (attacker.AttacksRemaining == 0) AdvanceTurn(scene, attacker);
                    await playthroughRepository.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                    return Result<SceneAttackResultDto>.Ok(new SceneAttackResultDto
                    {
                        IsSupport = true, HealthRestored = healedHp - oldHp,
                        MagicRestored = healedMp - oldMp, TargetCurrentHp = healedHp
                    });
                }

                baseDamage = ScenePlaythroughEquipmentEffects.Apply(
                    spell.DamageEffect!.Value,
                    attackerEquipment.GetSpellDamageModifier(
                        spell.PlaythroughSpellTypeId));
                attackLabel = spell.Name;
                break;

            default:
                return InvalidAttackType("The selected attack type is invalid.");
        }

        var damageReduction = attackType switch
        {
            SceneAttackType.Melee => targetEquipment.MeleeDamageReduction,
            SceneAttackType.Range => targetEquipment.BowDamageReduction,
            SceneAttackType.Spell => targetEquipment.SpellDamageReduction,
            _ => 0
        };
        var damage = ScenePlaythroughEquipmentEffects.Apply(
            baseDamage,
            roll - damageReduction);
        var targetCurrentHp = targetJourneyCharacter?.CurrentHp
            ?? targetSceneCharacter?.CurrentHp
            ?? 0;
        var remainingHp = Math.Max(0, targetCurrentHp - damage);

        if (targetJourneyCharacter is not null)
            targetJourneyCharacter.CurrentHp = remainingHp;
        else
            targetSceneCharacter!.CurrentHp = remainingHp;

        AddEvent(
            scene,
            $"{attackerName} hit {targetName} with {attackLabel} for {damage} damage");

        var targetDefeated = remainingHp == 0;
        if (targetDefeated && targetJourneyCharacter is not null)
        {
            targetJourneyCharacter.IsDown = true;
            target.DownedTurnsRemaining = DownedScheduledTurns;
            AddEvent(scene, $"{targetName} was downed");
        }
        else if (targetDefeated && targetSceneCharacter is not null)
        {
            targetSceneCharacter.IsDead = true;
            targetSceneCharacter.IsActive = false;
            target.IsActive = false;
            scene.SceneParticipants.Remove(target);
            AddEvent(scene, $"{targetName} was defeated");
        }

        string? rewardStat = null;
        var rewardAmount = 0;
        if (targetDefeated &&
            attacker.ParticipantType == ParticipantType.Player &&
            target.ParticipantType == ParticipantType.Enemy &&
            attackerJourneyCharacter is not null)
        {
            var rewardHp = RandomNumberGenerator.GetInt32(2) == 0;
            if (rewardHp)
            {
                rewardStat = "HP";
                var previousHp = attackerJourneyCharacter.CurrentHp;
                var effectiveMaxHp = ScenePlaythroughEquipmentEffects.Apply(
                    attackerJourneyCharacter.MaxHp,
                    attackerEquipment.MaxHpModifier,
                    minimum: 1);
                attackerJourneyCharacter.CurrentHp = (int)Math.Min(
                    effectiveMaxHp,
                    (long)previousHp + 4);
                rewardAmount = attackerJourneyCharacter.CurrentHp - previousHp;
            }
            else
            {
                rewardStat = "MP";
                var previousMp = attackerJourneyCharacter.CurrentMp;
                var effectiveMaxMp = ScenePlaythroughEquipmentEffects.Apply(
                    attackerJourneyCharacter.MaxMp,
                    attackerEquipment.MaxMpModifier);
                attackerJourneyCharacter.CurrentMp = (int)Math.Min(
                    effectiveMaxMp,
                    (long)previousMp + 4);
                rewardAmount = attackerJourneyCharacter.CurrentMp - previousMp;
            }

            AddEvent(
                scene,
                rewardAmount > 0
                    ? $"{attackerName} gained {rewardAmount} {rewardStat}"
                    : $"{attackerName} received an {rewardStat} reward but was already at maximum");
        }

        if (isCounterattack)
        {
            ClearCounterattack(scene);
            AdvanceTurn(scene, target);
        }
        else
        {
            attacker.AttacksRemaining--;
            if (!targetDefeated)
            {
                scene.CounterattackerId = target.Id;
                scene.CounterattackTargetId = attacker.Id;
                scene.CounterattackToken = Guid.NewGuid();
                AddEvent(scene, $"{targetName} may counterattack {attackerName}");
            }
            else if (attacker.AttacksRemaining == 0)
                AdvanceTurn(scene, attacker);
        }

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result<SceneAttackResultDto>.Ok(new SceneAttackResultDto
        {
            Damage = damage,
            TargetCurrentHp = remainingHp,
            TargetDefeated = targetDefeated,
            RewardStat = rewardStat,
            RewardAmount = rewardAmount
        });

        static Result<SceneAttackResultDto> InvalidAttackType(string message) =>
            Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.InvalidAttackType", message));

        static Result<SceneAttackResultDto> AttackUnavailable(string message) =>
            Result<SceneAttackResultDto>.Fail(new Error(
                "ScenePlaythrough.AttackUnavailable", message));
    }

    public async Task<Result<SceneOpenChestResultDto>> OpenChestAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int chestId,
        int roll,
        CancellationToken ct)
    {
        if (roll is < 1 or > 6)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.InvalidChestRoll",
                "The chest roll must be between 1 and 6."));
        }

        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result<SceneOpenChestResultDto>.Fail(stateError);

        var participant = scene!.SceneParticipants.SingleOrDefault(
            candidate => candidate.Id == participantId);
        if (participant is null)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The scene participant was not found."));
        }

        if (scene.CurrentParticipantId != participant.Id || !participant.IsActive)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.NotCurrentTurn",
                "Only the active current participant can open a chest."));
        }

        var character = participant.JourneyPlaythroughCharacter;
        if (participant.ParticipantType != ParticipantType.Player || character is null)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.ChestUnavailable",
                "Only a player can open a chest."));
        }

        if (character.IsDown)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.ChestUnavailable",
                "A downed player cannot open a chest."));
        }

        var chest = scene.SceneChests.SingleOrDefault(candidate => candidate.Id == chestId);
        if (chest is null)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.ChestNotFound",
                "The scene chest was not found."));
        }

        if (chest.Status != ChestStatus.Unopened)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.ChestAlreadyOpened",
                "This chest has already been opened."));
        }

        if (roll > chest.DieSides)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.InvalidChestRoll",
                $"The selected chest uses a d{chest.DieSides}."));
        }

        var lootEntry = chest.ChestLootEntries
            .Where(entry => roll >= entry.RollMinimum && roll <= entry.RollMaximum)
            .OrderBy(entry => entry.RollMinimum)
            .ThenBy(entry => entry.Id)
            .FirstOrDefault();
        if (lootEntry is null)
        {
            return Result<SceneOpenChestResultDto>.Fail(new Error(
                "ScenePlaythrough.ChestLootNotConfigured",
                $"No loot is configured for a roll of {roll}."));
        }

        var isEquippable = lootEntry.PlaythroughEquippableItem is not null;
        var item = isEquippable
            ? lootEntry.PlaythroughEquippableItem!.ToLootItemDto()
            : lootEntry.PlaythroughConsumableItem!.ToLootItemDto();

        var currentInventoryCount = isEquippable
            ? character.EquippableItems.Count
            : character.ConsumableItems.Count(itemLink => !itemLink.IsUsed);
        var equipmentEffects = ScenePlaythroughEquipmentEffects.For(participant);
        var inventoryLimit = isEquippable
            ? ScenePlaythroughEquipmentEffects.Apply(
                character.MaxEquippableInventory,
                equipmentEffects.MaxEquippableInventoryModifier)
            : ScenePlaythroughEquipmentEffects.Apply(
                character.MaxConsumableInventory,
                equipmentEffects.MaxConsumableInventoryModifier);
        if ((long)currentInventoryCount + lootEntry.Quantity > inventoryLimit)
        {
            var inventoryType = isEquippable ? "equippable" : "consumable";
            AddEvent(
                scene,
                $"{character.PlaythroughCharacter.Name} rolled {lootEntry.Quantity} x {item.Name} from {chest.Name}, but their {inventoryType} inventory was full and they forfeited their action");
            AdvanceTurn(scene, participant);

            await playthroughRepository.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result<SceneOpenChestResultDto>.Ok(new SceneOpenChestResultDto
            {
                ChestId = chest.Id,
                ChestName = chest.Name,
                Roll = roll,
                Quantity = lootEntry.Quantity,
                IsEquippable = isEquippable,
                Awarded = false,
                Item = item
            });
        }

        for (var quantity = 0; quantity < lootEntry.Quantity; quantity++)
        {
            if (isEquippable)
            {
                character.EquippableItems.Add(new JourneyPTCharacterEquippableItem
                {
                    IsEquipped = true,
                    PlaythroughEquippableItemId = item.Id
                });
            }
            else
            {
                character.ConsumableItems.Add(new JourneyPTCharacterConsumableItem
                {
                    IsUsed = false,
                    PlaythroughConsumableItemId = item.Id
                });
            }
        }

        chest.Status = ChestStatus.Opened;
        chest.RolledValue = roll;
        chest.OpenedAt = DateTime.UtcNow;
        chest.SelectedLootEntry = lootEntry;
        chest.SelectedLootEntryId = lootEntry.Id;

        var characterName = character.PlaythroughCharacter.Name;
        AddEvent(
            scene,
            $"{characterName} opened {chest.Name} with a roll of {roll} and received {lootEntry.Quantity} x {item.Name}");
        AdvanceTurn(scene, participant);

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result<SceneOpenChestResultDto>.Ok(new SceneOpenChestResultDto
        {
            ChestId = chest.Id,
            ChestName = chest.Name,
            Roll = roll,
            Quantity = lootEntry.Quantity,
            IsEquippable = isEquippable,
            Awarded = true,
            Item = item
        });
    }

    public async Task<Result<SceneUseConsumableResultDto>> UseConsumableAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int inventoryItemId,
        CancellationToken ct)
    {
        await using var transaction =
            await playthroughRepository.BeginSceneStartTransactionAsync(ct);
        var scene = await playthroughRepository.GetSceneForCharacterInstanceAddAsync(
            userId, playthroughId, sceneId, ct);
        var stateError = ValidateManageableScene(scene);
        if (stateError is not null)
            return Result<SceneUseConsumableResultDto>.Fail(stateError);

        var participant = scene!.SceneParticipants.SingleOrDefault(
            candidate => candidate.Id == participantId);
        if (participant is null)
        {
            return Result<SceneUseConsumableResultDto>.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The scene participant was not found."));
        }

        if (scene.CurrentParticipantId != participant.Id || !participant.IsActive)
        {
            return Result<SceneUseConsumableResultDto>.Fail(new Error(
                "ScenePlaythrough.NotCurrentTurn",
                "Only the active current participant can use a consumable item."));
        }

        if (participant.JourneyPlaythroughCharacter?.IsDown == true ||
            participant.ScenePlaythroughCharacter?.IsDead == true)
        {
            return Result<SceneUseConsumableResultDto>.Fail(new Error(
                "ScenePlaythrough.UseConsumableUnavailable",
                "A downed or defeated participant cannot use a consumable item."));
        }

        PlaythroughConsumableItem? item;
        int previousHp;
        int previousMp;
        int currentHp;
        int currentMp;
        int maxHp;
        int maxMp;

        if (participant.JourneyPlaythroughCharacter is { } journeyCharacter)
        {
            var itemLink = journeyCharacter.ConsumableItems.SingleOrDefault(
                link => link.Id == inventoryItemId && !link.IsUsed);
            if (itemLink is null)
            {
                return Result<SceneUseConsumableResultDto>.Fail(new Error(
                    "ScenePlaythrough.ConsumableNotFound",
                    "The available consumable inventory item was not found."));
            }

            item = itemLink.PlaythroughConsumableItem;
            var equipmentEffects = ScenePlaythroughEquipmentEffects.For(journeyCharacter);
            maxHp = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter.MaxHp, equipmentEffects.MaxHpModifier);
            maxMp = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter.MaxMp, equipmentEffects.MaxMpModifier);
            previousHp = journeyCharacter.CurrentHp;
            previousMp = journeyCharacter.CurrentMp;
            currentHp = RestoreStat(previousHp, item.HpEffect, maxHp);
            currentMp = RestoreStat(previousMp, item.MpEffect, maxMp);
            journeyCharacter.CurrentHp = currentHp;
            journeyCharacter.CurrentMp = currentMp;
            itemLink.IsUsed = true;
        }
        else if (participant.ScenePlaythroughCharacter is { } sceneCharacter)
        {
            var itemLink = sceneCharacter.ConsumableItems.SingleOrDefault(
                link => link.Id == inventoryItemId && !link.IsUsed);
            if (itemLink is null)
            {
                return Result<SceneUseConsumableResultDto>.Fail(new Error(
                    "ScenePlaythrough.ConsumableNotFound",
                    "The available consumable inventory item was not found."));
            }

            item = itemLink.PlaythroughConsumableItem;
            var equipmentEffects = ScenePlaythroughEquipmentEffects.For(sceneCharacter);
            maxHp = ScenePlaythroughEquipmentEffects.Apply(
                sceneCharacter.MaxHp, equipmentEffects.MaxHpModifier);
            maxMp = ScenePlaythroughEquipmentEffects.Apply(
                sceneCharacter.MaxMp, equipmentEffects.MaxMpModifier);
            previousHp = sceneCharacter.CurrentHp;
            previousMp = sceneCharacter.CurrentMp;
            currentHp = RestoreStat(previousHp, item.HpEffect, maxHp);
            currentMp = RestoreStat(previousMp, item.MpEffect, maxMp);
            sceneCharacter.CurrentHp = currentHp;
            sceneCharacter.CurrentMp = currentMp;
            itemLink.IsUsed = true;
        }
        else
        {
            return Result<SceneUseConsumableResultDto>.Fail(new Error(
                "ScenePlaythrough.UseConsumableUnavailable",
                "This participant does not have a usable inventory."));
        }

        var hpRestored = currentHp - previousHp;
        var mpRestored = currentMp - previousMp;
        var characterName = participant.JourneyPlaythroughCharacter
                ?.PlaythroughCharacter.Name
            ?? participant.ScenePlaythroughCharacter?.PlaythroughCharacter.Name
            ?? "Unknown character";
        AddEvent(
            scene,
            $"{characterName} used {item.Name} and restored {hpRestored} HP and {mpRestored} MP");
        AdvanceTurn(scene, participant);

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result<SceneUseConsumableResultDto>.Ok(new SceneUseConsumableResultDto
        {
            InventoryItemId = inventoryItemId,
            ItemId = item.Id,
            ItemName = item.Name,
            HpRestored = hpRestored,
            MpRestored = mpRestored,
            CurrentHp = currentHp,
            MaxHp = maxHp,
            CurrentMp = currentMp,
            MaxMp = maxMp
        });
    }

    public async Task<Result> TradeItemAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int targetParticipantId,
        int inventoryItemId,
        bool isEquippable,
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
            candidate => candidate.Id == participantId);
        var targetParticipant = scene.SceneParticipants.SingleOrDefault(
            candidate => candidate.Id == targetParticipantId);
        if (participant is null || targetParticipant is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "A trade participant was not found."));
        }

        if (scene.CurrentParticipantId != participant.Id || !participant.IsActive)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.NotCurrentTurn",
                "Only the active current participant can trade an item."));
        }

        var character = participant.JourneyPlaythroughCharacter;
        var targetCharacter = targetParticipant.JourneyPlaythroughCharacter;
        if (participant.ParticipantType != ParticipantType.Player ||
            targetParticipant.ParticipantType != ParticipantType.Player ||
            character is null ||
            targetCharacter is null ||
            targetParticipant.Id == participant.Id ||
            !targetParticipant.IsActive ||
            character.IsDown)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.TradeUnavailable",
                "Items can only be traded between two active player participants."));
        }

        string itemName;
        string sourceName;
        string destinationName;

        if (isEquippable)
        {
            var itemLink = character.EquippableItems.SingleOrDefault(
                link => link.Id == inventoryItemId);
            var sourceCharacter = character;
            var destinationCharacter = targetCharacter;

            if (itemLink is null)
            {
                itemLink = targetCharacter.EquippableItems.SingleOrDefault(
                    link => link.Id == inventoryItemId);
                sourceCharacter = targetCharacter;
                destinationCharacter = character;
            }

            if (itemLink is null)
            {
                return Result.Fail(new Error(
                    "ScenePlaythrough.TradeItemNotFound",
                    "The equippable inventory item was not found."));
            }

            var destinationEquipment =
                ScenePlaythroughEquipmentEffects.For(destinationCharacter);
            var destinationLimit = ScenePlaythroughEquipmentEffects.Apply(
                destinationCharacter.MaxEquippableInventory,
                destinationEquipment.MaxEquippableInventoryModifier);
            if (destinationCharacter.EquippableItems.Count >= destinationLimit)
            {
                return Result.Fail(new Error(
                    "ScenePlaythrough.TradeInventoryFull",
                    "The receiving player's equippable inventory is full."));
            }

            sourceCharacter.EquippableItems.Remove(itemLink);
            itemLink.JourneyPTCharacterId = destinationCharacter.Id;
            itemLink.JourneyPTCharacter = destinationCharacter;
            destinationCharacter.EquippableItems.Add(itemLink);

            itemName = itemLink.PlaythroughEquippableItem.Name;
            sourceName = sourceCharacter.PlaythroughCharacter.Name;
            destinationName = destinationCharacter.PlaythroughCharacter.Name;
        }
        else
        {
            var itemLink = character.ConsumableItems.SingleOrDefault(
                link => link.Id == inventoryItemId && !link.IsUsed);
            var sourceCharacter = character;
            var destinationCharacter = targetCharacter;

            if (itemLink is null)
            {
                itemLink = targetCharacter.ConsumableItems.SingleOrDefault(
                    link => link.Id == inventoryItemId && !link.IsUsed);
                sourceCharacter = targetCharacter;
                destinationCharacter = character;
            }

            if (itemLink is null)
            {
                return Result.Fail(new Error(
                    "ScenePlaythrough.TradeItemNotFound",
                    "The consumable inventory item was not found."));
            }

            var destinationEquipment =
                ScenePlaythroughEquipmentEffects.For(destinationCharacter);
            var destinationLimit = ScenePlaythroughEquipmentEffects.Apply(
                destinationCharacter.MaxConsumableInventory,
                destinationEquipment.MaxConsumableInventoryModifier);
            if (destinationCharacter.ConsumableItems.Count(link => !link.IsUsed) >=
                destinationLimit)
            {
                return Result.Fail(new Error(
                    "ScenePlaythrough.TradeInventoryFull",
                    "The receiving player's consumable inventory is full."));
            }

            sourceCharacter.ConsumableItems.Remove(itemLink);
            itemLink.JourneyPTCharacterId = destinationCharacter.Id;
            itemLink.JourneyPTCharacter = destinationCharacter;
            destinationCharacter.ConsumableItems.Add(itemLink);

            itemName = itemLink.PlaythroughConsumableItem.Name;
            sourceName = sourceCharacter.PlaythroughCharacter.Name;
            destinationName = destinationCharacter.PlaythroughCharacter.Name;
        }

        AddEvent(scene, $"{sourceName} traded {itemName} to {destinationName}");
        AdvanceTurn(scene, participant);

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Ok();
    }

    public async Task<Result> TransformAsync(
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
            candidate => candidate.Id == participantId);
        if (participant is null)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.ParticipantNotFound",
                "The scene participant was not found."));
        }

        if (scene.CurrentParticipantId != participant.Id || !participant.IsActive)
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.NotCurrentTurn",
                "Only the active current participant can transform."));
        }

        var journeyCharacter = participant.JourneyPlaythroughCharacter;
        var sceneCharacter = participant.ScenePlaythroughCharacter;
        var alternateForm = journeyCharacter?.AlternateForm ?? sceneCharacter?.AlternateForm;
        var alternateFormId = journeyCharacter?.AlternateFormId ?? sceneCharacter?.AlternateFormId;
        var baseCharacter = journeyCharacter?.PlaythroughCharacter ?? sceneCharacter?.PlaythroughCharacter;
        if (baseCharacter is null || alternateForm is null || alternateFormId is null ||
            journeyCharacter is { IsDown: true } || sceneCharacter is { IsDead: true })
        {
            return Result.Fail(new Error(
                "ScenePlaythrough.TransformUnavailable",
                "This participant does not have an available alternate form."));
        }

        var wasInAlternateForm = journeyCharacter?.IsInAlternateForm
            ?? sceneCharacter!.IsInAlternateForm;
        if (journeyCharacter is not null)
            journeyCharacter.IsInAlternateForm = !wasInAlternateForm;
        else
            sceneCharacter!.IsInAlternateForm = !wasInAlternateForm;

        AddEvent(scene, wasInAlternateForm
            ? $"{alternateForm.Name} returned to {baseCharacter.Name}"
            : $"{baseCharacter.Name} transformed into {alternateForm.Name}");
        AdvanceTurn(scene, participant);

        await playthroughRepository.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Result.Ok();
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

    private static Error? ExecuteSceneEventAction(
        ScenePT scene,
        ScenePTActionEvent action)
    {
        return action.EventActionType switch
        {
            EventActionType.CharacterStatAdjustment =>
                ExecuteCharacterStatAdjustment(scene, action),
            EventActionType.CharacterAddSpell =>
                ExecuteCharacterAddSpell(scene, action),
            EventActionType.CharacterChangeAlternateForm or EventActionType.CharacterClearAlternateForm =>
                ExecuteCharacterChangeAlternateForm(scene, action),
            EventActionType.CharacterInAlternateForm =>
                ExecuteCharacterInAlternateForm(scene, action),
            _ => EventExecutionError(
                action,
                $"Action type '{action.EventActionType}' is not supported.")
        };
    }

    private static Error? ExecuteCharacterChangeAlternateForm(
        ScenePT scene,
        ScenePTActionEvent action)
    {
        var change = action.CharacterChangeAlternateFormAction;
        var clearsForm = action.EventActionType == EventActionType.CharacterClearAlternateForm;

        if (change is null || (!clearsForm && (change.AlternateForm is null || change.AlternateFormId is null or <= 0)))
            return EventExecutionError(action, "The alternate form is missing.");
        if (clearsForm && change.AlternateFormId is not null)
            return EventExecutionError(action, "A clear-alternate-form action cannot specify an alternate character.");
        if (action.ActionTargetType is not (ActionTargetType.AllJourneyCharacters or ActionTargetType.SingleJourneyCharacter))
            return EventExecutionError(action, "Alternate forms can only be changed for journey characters.");

        var targets = scene.Playthrough.JourneyCharacters
            .Where(character => action.ActionTargetType == ActionTargetType.AllJourneyCharacters ||
                character.PlaythroughCharacterId == change.PlaythroughCharacterId)
            .ToList();

        if (action.ActionTargetType == ActionTargetType.AllJourneyCharacters && change.PlaythroughCharacterId is not null)
            return EventExecutionError(action, "An all-journey-characters action cannot specify a character.");
        if (action.ActionTargetType == ActionTargetType.SingleJourneyCharacter && targets.Count != 1)
            return EventExecutionError(action, "The targeted journey character is missing or ambiguous.");
        if (targets.Any(character => character.PlaythroughCharacterId == change.AlternateFormId))
            return EventExecutionError(action, "A character cannot be its own alternate form.");

        foreach (var character in targets)
        {
            character.AlternateFormId = change.AlternateFormId;
            character.AlternateForm = change.AlternateForm;
            if (clearsForm)
            {
                character.AlternateFormId = null;
                character.AlternateForm = null;
                character.IsInAlternateForm = false;
                AddEvent(scene, $"{character.PlaythroughCharacter.Name}'s alternate form was cleared");
            }
            else
                AddEvent(scene, $"{character.PlaythroughCharacter.Name}'s alternate form changed to {change.AlternateForm!.Name}");
        }
        return null;
    }

    private static Error? ExecuteCharacterInAlternateForm(
    ScenePT scene,
    ScenePTActionEvent action)
    {
        if (action.ActionTargetType is not (ActionTargetType.AllJourneyCharacters or ActionTargetType.SingleJourneyCharacter))
            return EventExecutionError(action, "Alternate forms can only be changed for journey characters.");

        if(action.ActionTargetType == ActionTargetType.AllJourneyCharacters && action.CharacterInAlternateFormAction?.PlaythroughCharacterId is not null)
            return EventExecutionError(action, "Cannot link a specific character to transform when the target type is set to all journey characters.");

        var allCharacters = action.ActionTargetType == ActionTargetType.AllJourneyCharacters;

        if (allCharacters)
        {
            scene.Playthrough.JourneyCharacters.ToList().ForEach(character =>
            {
                if(character.AlternateForm is not null)
                    character.IsInAlternateForm = true;
            });

            AddEvent(scene, $"Journey Characters that are able, are now in their alternate forms.");

            return null;
        }
            
        if(action.CharacterInAlternateFormAction is null || action.CharacterInAlternateFormAction.PlaythroughCharacterId is null)
            return EventExecutionError(action, "The targeted journey character is missing, cannot transform to alternate form.");

        var character = scene.Playthrough.JourneyCharacters.SingleOrDefault(character => character.PlaythroughCharacterId == action.CharacterInAlternateFormAction.PlaythroughCharacterId);

        if (character is null)
            return EventExecutionError(action, "The target character does not exist, cannot transform to alternate form.");

        if(character.AlternateForm is null)
            return EventExecutionError(action, "The target character does not have an alternate form, cannot transform to alternate form.");

        character.IsInAlternateForm = true;
        AddEvent(scene, $"{character.PlaythroughCharacter.Name} is now in their alternate form.");

        return null;
    }

    private static Error? ExecuteCharacterStatAdjustment(
        ScenePT scene,
        ScenePTActionEvent action)
    {
        var adjustment = action.CharacterStatAdjustmentAction;

        if (adjustment is null)
        {
            return EventExecutionError(
                action,
                "The character-stat adjustment payload is missing.");
        }

        if (!Enum.IsDefined(adjustment.CharacterStatType) ||
            !Enum.IsDefined(adjustment.AdjustmentOperation))
        {
            return EventExecutionError(
                action,
                "The character-stat adjustment contains an invalid type or operation.");
        }

        return ApplyToEventTargets(
            scene,
            action,
            adjustment.PlaythroughCharacterId,
            character => ApplyStatAdjustment(character, adjustment),
            character => ApplyStatAdjustment(character, adjustment));
    }


    private static Error? ExecuteCharacterAddSpell(
        ScenePT scene,
        ScenePTActionEvent action)
    {
        var addSpell = action.CharacterAddSpellAction;

        if (addSpell is null)
        {
            return EventExecutionError(
                action,
                "The character-add-spell payload is missing.");
        }

        if (addSpell.PlaythroughSpellId <= 0 || addSpell.PlaythroughSpell is null)
        {
            return EventExecutionError(
                action,
                "The spell target is invalid.");
        }

        return ApplyToEventTargets(
            scene,
            action,
            addSpell.PlaythroughCharacterId,
            character =>
            {
                if (character.Spells.All(item =>
                    item.PlaythroughSpellId != addSpell.PlaythroughSpellId))
                {
                    character.Spells.Add(new JourneyPTCharacterSpell
                    {
                        SourceJourneyCharacterSpellId = null,
                        PlaythroughSpellId = addSpell.PlaythroughSpellId,
                        PlaythroughSpell = addSpell.PlaythroughSpell
                    });
                    AddEvent(scene, $"{character.PlaythroughCharacter.Name} learned {addSpell.PlaythroughSpell.Name}");
                }
            },
            character =>
            {
                if (character.Spells.All(item =>
                    item.PlaythroughSpellId != addSpell.PlaythroughSpellId))
                {
                    character.Spells.Add(new ScenePTCharacterSpell
                    {
                        SourceSceneCharacterSpellId = null,
                        PlaythroughSpellId = addSpell.PlaythroughSpellId,
                        PlaythroughSpell = addSpell.PlaythroughSpell
                    });
                    AddEvent(scene, $"{character.PlaythroughCharacter.Name} learned {addSpell.PlaythroughSpell.Name}");
                }
            });
    }

    private static Error? ApplyToEventTargets(
        ScenePT scene,
        ScenePTActionEvent action,
        int? playthroughCharacterId,
        Action<JourneyPTCharacter> applyToJourneyCharacter,
        Action<ScenePTCharacter> applyToSceneCharacter)
    {
        switch (action.ActionTargetType)
        {
            case ActionTargetType.AllJourneyCharacters:
                if (playthroughCharacterId is not null)
                {
                    return EventExecutionError(
                        action,
                        "An all-journey-characters action cannot specify a character.");
                }

                foreach (var character in scene.Playthrough.JourneyCharacters)
                    applyToJourneyCharacter(character);
                return null;

            case ActionTargetType.SingleJourneyCharacter:
            {
                var targets = playthroughCharacterId is int characterId
                    ? scene.Playthrough.JourneyCharacters.Where(character =>
                        character.PlaythroughCharacterId == characterId).ToList()
                    : [];

                if (targets.Count != 1)
                {
                    return EventExecutionError(
                        action,
                        "The targeted journey character is missing or ambiguous.");
                }

                applyToJourneyCharacter(targets[0]);
                return null;
            }

            case ActionTargetType.AllSceneCharacters:
                if (playthroughCharacterId is not null)
                {
                    return EventExecutionError(
                        action,
                        "An all-scene-characters action cannot specify a character.");
                }

                foreach (var character in scene.SceneCharacters)
                    applyToSceneCharacter(character);
                return null;

            case ActionTargetType.SingleSceneCharacter:
            {
                var targets = playthroughCharacterId is int characterId
                    ? scene.SceneCharacters.Where(character =>
                        character.PlaythroughCharacterId == characterId).ToList()
                    : [];

                if (targets.Count != 1)
                {
                    return EventExecutionError(
                        action,
                        "The targeted scene character is missing or ambiguous.");
                }

                applyToSceneCharacter(targets[0]);
                return null;
            }

            default:
                return EventExecutionError(
                    action,
                    $"Target type '{action.ActionTargetType}' is not supported.");
        }
    }

    private static void ApplyStatAdjustment(
        JourneyPTCharacter character,
        PTCharacterStatAdjustmentAction adjustment)
    {
        switch (adjustment.CharacterStatType)
        {
            case CharacterStatType.CurrentHp:
                character.CurrentHp = ApplyBoundedAdjustment(
                    character.CurrentHp,
                    adjustment,
                    0,
                    character.MaxHp);
                character.IsDown = character.CurrentHp == 0;
                break;
            case CharacterStatType.CurrentMp:
                character.CurrentMp = ApplyBoundedAdjustment(
                    character.CurrentMp,
                    adjustment,
                    0,
                    character.MaxMp);
                break;
            case CharacterStatType.MaxHp:
                character.MaxHp = ApplyBoundedAdjustment(
                    character.MaxHp,
                    adjustment,
                    1,
                    int.MaxValue);
                character.CurrentHp = Math.Min(character.CurrentHp, character.MaxHp);
                character.IsDown = character.CurrentHp == 0;
                break;
            case CharacterStatType.MaxMp:
                character.MaxMp = ApplyBoundedAdjustment(
                    character.MaxMp,
                    adjustment,
                    0,
                    int.MaxValue);
                character.CurrentMp = Math.Min(character.CurrentMp, character.MaxMp);
                break;
            case CharacterStatType.Movement:
                character.Movement = ApplyBoundedAdjustment(
                    character.Movement,
                    adjustment,
                    int.MinValue,
                    int.MaxValue);
                break;
            case CharacterStatType.MeleeAttackDamage:
                character.MeleeAttackDamage = ApplyNullableAttackAdjustment(
                    character.MeleeAttackDamage,
                    adjustment);
                break;
            case CharacterStatType.BowAttackDamage:
                character.BowAttackDamage = ApplyNullableAttackAdjustment(
                    character.BowAttackDamage,
                    adjustment);
                break;
        }
    }

    private static void ApplyStatAdjustment(
        ScenePTCharacter character,
        PTCharacterStatAdjustmentAction adjustment)
    {
        switch (adjustment.CharacterStatType)
        {
            case CharacterStatType.CurrentHp:
                character.CurrentHp = ApplyBoundedAdjustment(
                    character.CurrentHp,
                    adjustment,
                    0,
                    character.MaxHp);
                character.IsDead = character.CurrentHp == 0;
                break;
            case CharacterStatType.CurrentMp:
                character.CurrentMp = ApplyBoundedAdjustment(
                    character.CurrentMp,
                    adjustment,
                    0,
                    character.MaxMp);
                break;
            case CharacterStatType.MaxHp:
                character.MaxHp = ApplyBoundedAdjustment(
                    character.MaxHp,
                    adjustment,
                    1,
                    int.MaxValue);
                character.CurrentHp = Math.Min(character.CurrentHp, character.MaxHp);
                character.IsDead = character.CurrentHp == 0;
                break;
            case CharacterStatType.MaxMp:
                character.MaxMp = ApplyBoundedAdjustment(
                    character.MaxMp,
                    adjustment,
                    0,
                    int.MaxValue);
                character.CurrentMp = Math.Min(character.CurrentMp, character.MaxMp);
                break;
            case CharacterStatType.Movement:
                character.Movement = ApplyBoundedAdjustment(
                    character.Movement,
                    adjustment,
                    int.MinValue,
                    int.MaxValue);
                break;
            case CharacterStatType.MeleeAttackDamage:
                character.MeleeAttackDamage = ApplyNullableAttackAdjustment(
                    character.MeleeAttackDamage,
                    adjustment);
                break;
            case CharacterStatType.BowAttackDamage:
                character.BowAttackDamage = ApplyNullableAttackAdjustment(
                    character.BowAttackDamage,
                    adjustment);
                break;
        }
    }

    private static int? ApplyNullableAttackAdjustment(
        int? currentValue,
        PTCharacterStatAdjustmentAction adjustment)
    {
        if (currentValue is null &&
            adjustment.AdjustmentOperation != AdjustmentOperation.Set)
        {
            return null;
        }

        return ApplyBoundedAdjustment(
            currentValue ?? 0,
            adjustment,
            int.MinValue,
            int.MaxValue);
    }

    private static int ApplyBoundedAdjustment(
        int currentValue,
        PTCharacterStatAdjustmentAction adjustment,
        int minimum,
        int maximum)
    {
        var adjusted = adjustment.AdjustmentOperation switch
        {
            AdjustmentOperation.Add =>
                (long)currentValue + adjustment.Value,
            AdjustmentOperation.Subtract =>
                (long)currentValue - adjustment.Value,
            AdjustmentOperation.Set => adjustment.Value,
            AdjustmentOperation.Multiply =>
                (long)currentValue * adjustment.Value,
            _ => currentValue
        };

        return (int)Math.Clamp(adjusted, minimum, maximum);
    }

    private static Error EventExecutionError(
        ScenePTActionEvent action,
        string reason) =>
        new(
            "ScenePlaythrough.EventExecutionFailed",
            $"Scene event action '{action.Name}' could not be executed. {reason}");

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

    private static Error? ValidateManageableScene(ScenePT? scene, bool allowCounterattack = false)
    {
        if (scene is null)
            return new Error("ScenePlaythrough.NotFound", "Scene playthrough was not found.");
        if (scene.Playthrough.CompletedAt is not null)
            return new Error("Playthrough.Completed", "The playthrough is completed.");
        if (!allowCounterattack && scene.CounterattackToken is not null)
            return new Error("ScenePlaythrough.CounterattackPending", "Resolve or pass the pending counterattack first.");
        return scene.Status == ScenePlaythroughStatus.InProgress
            ? null
            : new Error(
                "ScenePlaythrough.NotInProgress",
                "Scene options are only available while the scene is in progress.");
    }

    private static int RestoreStat(int current, int effect, int maximum)
    {
        if (current >= maximum)
            return current;

        return (int)Math.Min((long)current + effect, maximum);
    }

    private static Error? ValidateChestInput(CreateScenePlaythroughChestDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 250)
        {
            return new Error(
                "ScenePlaythrough.InvalidChest",
                "A chest name of 250 characters or fewer is required.");
        }

        if (input.DieSides is < 1 or > 6)
        {
            return new Error(
                "ScenePlaythrough.InvalidChest",
                "A live chest must use between one and six die faces.");
        }

        if (input.LootEntries.Count == 0)
        {
            return new Error(
                "ScenePlaythrough.InvalidChestLoot",
                "Configure an item for every die face.");
        }

        var coveredRolls = new HashSet<int>();
        foreach (var entry in input.LootEntries)
        {
            if (entry.RollMinimum < 1 ||
                entry.RollMaximum < entry.RollMinimum ||
                entry.RollMaximum > input.DieSides)
            {
                return new Error(
                    "ScenePlaythrough.InvalidChestLoot",
                    $"Every loot range must be between 1 and {input.DieSides}.");
            }

            if (entry.Quantity < 1)
            {
                return new Error(
                    "ScenePlaythrough.InvalidChestLoot",
                    "Every chest quantity must be at least one.");
            }

            if (entry.PlaythroughEquippableItemId.HasValue ==
                entry.PlaythroughConsumableItemId.HasValue)
            {
                return new Error(
                    "ScenePlaythrough.InvalidChestLoot",
                    "Every die face must specify exactly one item.");
            }

            for (var roll = entry.RollMinimum; roll <= entry.RollMaximum; roll++)
            {
                if (!coveredRolls.Add(roll))
                {
                    return new Error(
                        "ScenePlaythrough.InvalidChestLoot",
                        "Chest loot ranges cannot overlap.");
                }
            }
        }

        return coveredRolls.Count == input.DieSides
            ? null
            : new Error(
                "ScenePlaythrough.InvalidChestLoot",
                "Configure an item for every die face.");
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
        currentParticipant.AttacksRemaining = 0;
        var participants = scene.SceneParticipants
            .Where(participant => participant.IsActive)
            .Append(currentParticipant)
            .DistinctBy(participant => participant.Id)
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

        for (var offset = 1; offset <= participants.Count; offset++)
        {
            var nextIndex = (currentIndex + offset) % participants.Count;
            if (nextIndex == 0)
                scene.RoundNumber++;

            var candidate = participants[nextIndex];
            if (!candidate.IsActive) continue;
            if (!PrepareForScheduledTurn(scene, candidate))
                continue;

            ResetAttacksForTurn(candidate);
            scene.CurrentParticipant = candidate;
            return;
        }

        scene.CurrentParticipant = null;
    }

    private static void ResetAttacksForTurn(ScenePTParticipant participant)
    {
        participant.AttacksRemaining =
            ScenePlaythroughEquipmentEffects.For(participant).GetAttacksPerTurn();
    }

    private static bool PrepareForScheduledTurn(
        ScenePT scene,
        ScenePTParticipant participant)
    {
        var journeyCharacter = participant.JourneyPlaythroughCharacter;
        if (journeyCharacter?.IsDown != true)
            return participant.ScenePlaythroughCharacter?.IsDead != true;

        var turnsRemaining = participant.DownedTurnsRemaining
            ?? DownedScheduledTurns;
        turnsRemaining--;
        participant.DownedTurnsRemaining = Math.Max(0, turnsRemaining);

        if (turnsRemaining > 0)
            return false;

        journeyCharacter.CurrentHp = 1;
        journeyCharacter.IsDown = false;
        participant.DownedTurnsRemaining = null;
        AddEvent(
            scene,
            $"{journeyCharacter.PlaythroughCharacter.Name} recovered with 1 HP");
        return true;
    }

    private static bool IsValidAttackTarget(
        ScenePTParticipant attacker,
        ScenePTParticipant target)
    {
        if (attacker.Id == target.Id || !target.IsActive)
            return false;

        if (target.JourneyPlaythroughCharacter?.IsDown == true ||
            target.ScenePlaythroughCharacter?.IsDead == true)
        {
            return false;
        }

        return attacker.ParticipantType == ParticipantType.Enemy
            ? target.ParticipantType is ParticipantType.Player or ParticipantType.NPC
            : target.ParticipantType == ParticipantType.Enemy;
    }

    private static IEnumerable<PlaythroughSpell> GetSpells(
        ScenePTParticipant participant)
    {
        var characterSpells = ScenePlaythroughSpellSelection.For(participant);
        var equipmentSpells = ScenePlaythroughEquipmentEffects.For(participant)
            .AddedSpells;

        return characterSpells
            .Concat(equipmentSpells)
            .GroupBy(spell => spell.Id)
            .Select(group => group.First());
    }
}
