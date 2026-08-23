using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughMappings
{
    public static ScenePlaythroughDetailsDto ToDetailsDto(this ScenePT scene)
    {
        return new ScenePlaythroughDetailsDto
        {
            Id = scene.Id,
            PlaythroughId = scene.PlaythroughId,
            Name = scene.Name,
            Description = scene.Description,
            PhotoUrl = scene.PhotoUrl,
            Status = scene.Status,
            RoundNumber = scene.RoundNumber,
            StartedAt = scene.StartedAt,
            EndedAt = scene.EndedAt,
            CurrentParticipantId = scene.CurrentParticipantId,
            Participants = scene.SceneParticipants
                .OrderBy(participant => participant.ParticipantType)
                .ThenBy(participant => participant.SortOrderWithinType)
                .ThenBy(participant => participant.Id)
                .Select(participant => participant.ToDto(scene.CurrentParticipantId))
                .ToList(),
            JourneyCharacters = scene.Playthrough.JourneyCharacters
                .OrderBy(character => character.SourceJourneyCharacterId)
                .Select(character => new ScenePlaythroughJourneyCharacterOptionDto
                {
                    Id = character.Id,
                    PlaythroughCharacterId = character.PlaythroughCharacterId,
                    Name = character.PlaythroughCharacter.Name,
                    PhotoUrl = character.PlaythroughCharacter.PhotoUrl,
                    PortraitUrl = character.PlaythroughCharacter.PortraitUrl,
                    IsActive = character.IsActive,
                    IsParticipant = scene.SceneParticipants.Any(participant =>
                        participant.JourneyPlaythroughCharacterId == character.Id)
                })
                .ToList(),
            PlaythroughCharacters = scene.Playthrough.Characters
                .Where(character =>
                    character.CharacterType == CharacterType.NPC ||
                    character.CharacterType == CharacterType.Enemy)
                .OrderBy(character => character.CharacterType)
                .ThenBy(character => character.Name)
                .ThenBy(character => character.Id)
                .Select(character => new ScenePlaythroughCharacterOptionDto
                {
                    Id = character.Id,
                    Name = character.Name,
                    Description = character.Description,
                    PhotoUrl = character.PhotoUrl,
                    PortraitUrl = character.PortraitUrl,
                    CharacterType = character.CharacterType
                })
                .ToList(),
            Chests = scene.SceneChests
                .OrderBy(chest => chest.SourceSceneChestId)
                .ThenBy(chest => chest.Id)
                .Select(chest => new ScenePlaythroughChestDto
                {
                    Id = chest.Id,
                    Name = chest.Name,
                    DieSides = chest.DieSides,
                    Status = chest.Status,
                    RolledValue = chest.RolledValue,
                    OpenedAt = chest.OpenedAt,
                    SelectedLootEntryId = chest.SelectedLootEntryId,
                    LootEntries = chest.ChestLootEntries
                        .OrderBy(entry => entry.RollMinimum)
                        .ThenBy(entry => entry.Id)
                        .Select(entry => new ScenePlaythroughChestLootEntryDto
                        {
                            Id = entry.Id,
                            RollMinimum = entry.RollMinimum,
                            RollMaximum = entry.RollMaximum,
                            Quantity = entry.Quantity,
                            EquippableItem = entry.PlaythroughEquippableItem is null
                                ? null
                                : entry.PlaythroughEquippableItem.ToLootItemDto(),
                            ConsumableItem = entry.PlaythroughConsumableItem is null
                                ? null
                                : entry.PlaythroughConsumableItem.ToLootItemDto()
                        })
                        .ToList()
                })
                .ToList(),
            Dialogs = scene.SceneDialogs
                .OrderBy(dialog => dialog.SourceSceneDialogId)
                .ThenBy(dialog => dialog.Id)
                .Select(dialog => new ScenePlaythroughDialogDto
                {
                    Id = dialog.Id,
                    Title = dialog.Title,
                    DialogPages = dialog.DialogPages
                        .OrderBy(page => page.OrderNum)
                        .ThenBy(page => page.Id)
                        .Select(page => new ScenePlaythroughDialogPageDto
                        {
                            Id = page.Id,
                            OrderNum = page.OrderNum,
                            PhotoUrl = page.PhotoUrl,
                            DialogPageSections = page.DialogPageSections
                                .OrderBy(section => section.OrderNum)
                                .ThenBy(section => section.Id)
                                .Select(section => new ScenePlaythroughDialogSectionDto
                                {
                                    Id = section.Id,
                                    OrderNum = section.OrderNum,
                                    ReadingText = section.ReadingText,
                                    IsNarrator = section.IsNarrator,
                                    Character = section.Character is null
                                        ? null
                                        : new ScenePlaythroughDialogCharacterDto
                                        {
                                            Id = section.Character.Id,
                                            Name = section.Character.Name,
                                            PhotoUrl = section.Character.PhotoUrl,
                                            PortraitUrl = section.Character.PortraitUrl,
                                            DialogActiveColor =
                                                section.Character.DialogActiveColor,
                                            DialogInActiveColor =
                                                section.Character.DialogInActiveColor
                                        }
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList(),
            EventLogs = scene.Playthrough.EventLogs
                .OrderBy(eventLog => eventLog.EventTime)
                .ThenBy(eventLog => eventLog.Id)
                .Select(eventLog => new PlaythroughEventLogDto
                {
                    Id = eventLog.Id,
                    Message = eventLog.Message,
                    EventTime = eventLog.EventTime
                })
                .ToList()
        };
    }

    public static ScenePlaythroughLootItemDto ToLootItemDto(
        this PlaythroughConsumableItem item)
    {
        return new ScenePlaythroughLootItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            PhotoUrl = item.PhotoUrl,
            HpEffect = item.HpEffect,
            MpEffect = item.MpEffect
        };
    }

    public static ScenePlaythroughLootItemDto ToLootItemDto(
        this PlaythroughEquippableItem item)
    {
        return new ScenePlaythroughLootItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            PhotoUrl = item.PhotoUrl,
            MeleeAttackDamageModifier = item.MeleeAttackDamageModifier,
            BowAttackDamageModifier = item.BowAttackDamageModifier,
            MovementModifier = item.MovementModifier,
            MaxHpModifier = item.MaxHpModifier,
            MaxMpModifier = item.MaxMpModifier,
            MaxConsumableInventoryModifier = item.MaxConsumableInventoryModifier,
            MaxEquippableInventoryModifier = item.MaxEquippableInventoryModifier,
            MeleeDamageReduction = item.MeleeDamageReduction,
            BowDamageReduction = item.BowDamageReduction,
            SpellDamageReduction = item.SpellDamageReduction,
            AffectedSpellType = item.AffectedSpellType?.TypeName,
            SpellDamageModifier = item.SpellDamageModifier,
            AddedSpells = item.AddedSpells
                .OrderBy(spell => spell.Name)
                .ThenBy(spell => spell.Id)
                .Select(spell => new ScenePlaythroughSpellDto
                {
                    Id = spell.Id,
                    Name = spell.Name,
                    Description = spell.Description,
                    MpCost = spell.MpCost,
                    DamageEffect = spell.DamageEffect
                })
                .ToList()
        };
    }

    private static ScenePlaythroughParticipantDto ToDto(
        this ScenePTParticipant participant,
        int? currentParticipantId)
    {
        var journeyCharacter = participant.JourneyPlaythroughCharacter;
        var sceneCharacter = participant.ScenePlaythroughCharacter;
        var baseCharacter = journeyCharacter?.PlaythroughCharacter
            ?? sceneCharacter?.PlaythroughCharacter
            ?? throw new InvalidOperationException(
                "A scene participant must reference a playthrough character.");
        var alternateForm = journeyCharacter?.AlternateForm
            ?? sceneCharacter?.AlternateForm;
        var isInAlternateForm = journeyCharacter?.IsInAlternateForm
            ?? sceneCharacter?.IsInAlternateForm
            ?? false;
        PlaythroughCharacter displayedCharacter =
            isInAlternateForm && alternateForm is not null
                ? alternateForm
                : baseCharacter;

        return new ScenePlaythroughParticipantDto
        {
            Id = participant.Id,
            ParticipantType = participant.ParticipantType,
            SortOrderWithinType = participant.SortOrderWithinType,
            IsActive = participant.IsActive,
            IsCurrentParticipant = participant.Id == currentParticipantId,
            JourneyPlaythroughCharacterId = participant.JourneyPlaythroughCharacterId,
            ScenePlaythroughCharacterId = participant.ScenePlaythroughCharacterId,
            PlaythroughCharacterId = baseCharacter.Id,
            DisplayedPlaythroughCharacterId = displayedCharacter.Id,
            Name = displayedCharacter.Name,
            Description = displayedCharacter.Description,
            PhotoUrl = displayedCharacter.PhotoUrl,
            PortraitUrl = displayedCharacter.PortraitUrl,
            CurrentHp = journeyCharacter?.CurrentHp ?? sceneCharacter!.CurrentHp,
            MaxHp = journeyCharacter?.MaxHp ?? sceneCharacter!.MaxHp,
            CurrentMp = journeyCharacter?.CurrentMp ?? sceneCharacter!.CurrentMp,
            MaxMp = journeyCharacter?.MaxMp ?? sceneCharacter!.MaxMp,
            Movement = journeyCharacter?.Movement ?? sceneCharacter!.Movement,
            MeleeAttackDamage = journeyCharacter?.MeleeAttackDamage
                ?? sceneCharacter!.MeleeAttackDamage,
            BowAttackDamage = journeyCharacter?.BowAttackDamage
                ?? sceneCharacter!.BowAttackDamage,
            IsDown = journeyCharacter?.IsDown ?? false,
            IsDead = sceneCharacter?.IsDead ?? false,
            IsInAlternateForm = isInAlternateForm,
            DownedTurnsRemaining = participant.DownedTurnsRemaining,
            MaxConsumableInventory = journeyCharacter?.MaxConsumableInventory
                ?? sceneCharacter!.MaxConsumableInventory,
            MaxEquippableInventory = journeyCharacter?.MaxEquippableInventory
                ?? sceneCharacter!.MaxEquippableInventory,
            Spells = (journeyCharacter?.Spells
                    .Select(link => link.PlaythroughSpell)
                ?? sceneCharacter?.Spells
                    .Select(link => link.PlaythroughSpell)
                ?? [])
                .OrderBy(spell => spell.Name)
                .ThenBy(spell => spell.Id)
                .Select(spell => new ScenePlaythroughSpellDto
                {
                    Id = spell.Id,
                    Name = spell.Name,
                    Description = spell.Description,
                    MpCost = spell.MpCost,
                    DamageEffect = spell.DamageEffect
                })
                .ToList(),
            ConsumableItems = journeyCharacter?.ConsumableItems
                .Where(link => !link.IsUsed)
                .OrderBy(link => link.PlaythroughConsumableItem.Name)
                .ThenBy(link => link.Id)
                .Select(link => new ScenePlaythroughInventoryItemDto
                {
                    InventoryItemId = link.Id,
                    IsEquippable = false,
                    IsEquipped = false,
                    Item = link.PlaythroughConsumableItem.ToLootItemDto()
                })
                .ToList() ?? [],
            EquippableItems = journeyCharacter?.EquippableItems
                .OrderByDescending(link => link.IsEquipped)
                .ThenBy(link => link.PlaythroughEquippableItem.Name)
                .ThenBy(link => link.Id)
                .Select(link => new ScenePlaythroughInventoryItemDto
                {
                    InventoryItemId = link.Id,
                    IsEquippable = true,
                    IsEquipped = link.IsEquipped,
                    Item = link.PlaythroughEquippableItem.ToLootItemDto()
                })
                .ToList() ?? []
        };
    }
}
