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
            AvailableConsumableItems = scene.Playthrough.ConsumableItems
                .OrderBy(item => item.Name)
                .ThenBy(item => item.Id)
                .Select(item => item.ToLootItemDto())
                .ToList(),
            AvailableEquippableItems = scene.Playthrough.EquippableItems
                .OrderBy(item => item.Name)
                .ThenBy(item => item.Id)
                .Select(item => item.ToLootItemDto())
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
                            PageType = page.PageType,
                            MediaUrl = page.MediaUrl,
                            MediaContentType = page.MediaContentType,
                            DialogPageSections = page.PageType == DialogPageType.Image
                                ? page.DialogPageSections
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
                                : []
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
            AdditionalAttacksPerTurn = item.AdditionalAttacksPerTurn,
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
                    DamageEffect = spell.DamageEffect,
                    HealthEffect = spell.HealthEffect,
                    MagicEffect = spell.MagicEffect,
                    IsSupport = spell.DamageEffect.GetValueOrDefault() <= 0 && (spell.HealthEffect > 0 || spell.MagicEffect > 0)
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
        var equipmentEffects = ScenePlaythroughEquipmentEffects.For(participant);

        return new ScenePlaythroughParticipantDto
        {
            Id = participant.Id,
            ParticipantType = participant.ParticipantType,
            SortOrderWithinType = participant.SortOrderWithinType,
            IsActive = participant.IsActive,
            IsCurrentParticipant = participant.Id == currentParticipantId,
            AttacksPerTurn = equipmentEffects.GetAttacksPerTurn(),
            AttacksRemaining = participant.Id == currentParticipantId
                ? Math.Min(
                    participant.AttacksRemaining,
                    equipmentEffects.GetAttacksPerTurn())
                : 0,
            JourneyPlaythroughCharacterId = participant.JourneyPlaythroughCharacterId,
            ScenePlaythroughCharacterId = participant.ScenePlaythroughCharacterId,
            PlaythroughCharacterId = baseCharacter.Id,
            DisplayedPlaythroughCharacterId = displayedCharacter.Id,
            Name = displayedCharacter.Name,
            Description = displayedCharacter.Description,
            PhotoUrl = displayedCharacter.PhotoUrl,
            PortraitUrl = displayedCharacter.PortraitUrl,
            CurrentHp = journeyCharacter?.CurrentHp ?? sceneCharacter!.CurrentHp,
            MaxHp = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter?.MaxHp ?? sceneCharacter!.MaxHp,
                equipmentEffects.MaxHpModifier,
                minimum: 1),
            CurrentMp = journeyCharacter?.CurrentMp ?? sceneCharacter!.CurrentMp,
            MaxMp = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter?.MaxMp ?? sceneCharacter!.MaxMp,
                equipmentEffects.MaxMpModifier),
            Movement = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter?.Movement ?? sceneCharacter!.Movement,
                equipmentEffects.MovementModifier),
            MeleeAttackDamage = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter is not null
                    ? journeyCharacter.MeleeAttackDamage
                    : sceneCharacter!.MeleeAttackDamage,
                equipmentEffects.MeleeAttackDamageModifier),
            BowAttackDamage = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter is not null
                    ? journeyCharacter.BowAttackDamage
                    : sceneCharacter!.BowAttackDamage,
                equipmentEffects.BowAttackDamageModifier),
            MeleeDamageReduction = Math.Max(
                0, equipmentEffects.MeleeDamageReduction),
            BowDamageReduction = Math.Max(
                0, equipmentEffects.BowDamageReduction),
            SpellDamageReduction = Math.Max(
                0, equipmentEffects.SpellDamageReduction),
            IsDown = journeyCharacter?.IsDown ?? false,
            IsDead = sceneCharacter?.IsDead ?? false,
            IsInAlternateForm = isInAlternateForm,
            CanTransform = alternateForm is not null,
            DownedTurnsRemaining = participant.DownedTurnsRemaining,
            MaxConsumableInventory = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter?.MaxConsumableInventory
                    ?? sceneCharacter!.MaxConsumableInventory,
                equipmentEffects.MaxConsumableInventoryModifier),
            MaxEquippableInventory = ScenePlaythroughEquipmentEffects.Apply(
                journeyCharacter?.MaxEquippableInventory
                    ?? sceneCharacter!.MaxEquippableInventory,
                equipmentEffects.MaxEquippableInventoryModifier),
            Spells = (journeyCharacter?.Spells
                    .Select(link => link.PlaythroughSpell)
                ?? sceneCharacter?.Spells
                    .Select(link => link.PlaythroughSpell)
                ?? [])
                .Concat(equipmentEffects.AddedSpells)
                .GroupBy(spell => spell.Id)
                .Select(group => group.First())
                .OrderBy(spell => spell.Name)
                .ThenBy(spell => spell.Id)
                .Select(spell => new ScenePlaythroughSpellDto
                {
                    Id = spell.Id,
                    Name = spell.Name,
                    Description = spell.Description,
                    MpCost = spell.MpCost,
                    HealthEffect = spell.HealthEffect,
                    MagicEffect = spell.MagicEffect,
                    IsSupport = spell.DamageEffect.GetValueOrDefault() <= 0 && (spell.HealthEffect > 0 || spell.MagicEffect > 0),
                    DamageEffect = spell.DamageEffect is int damageEffect
                        ? ScenePlaythroughEquipmentEffects.Apply(
                            damageEffect,
                            equipmentEffects.GetSpellDamageModifier(
                                spell.PlaythroughSpellTypeId))
                        : null
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
                    IsEquipped = true,
                    Item = link.PlaythroughEquippableItem.ToLootItemDto()
                })
                .ToList() ?? []
        };
    }
}
