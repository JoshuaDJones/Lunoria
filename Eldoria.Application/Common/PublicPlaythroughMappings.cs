using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Common;

public static class PublicPlaythroughMappings
{
    public static PublicPlaythroughSnapshotDto ToPublicSnapshotDto(
        this Playthrough playthrough)
    {
        var activeScenes = playthrough.Scenes
            .Where(scene => scene.Status == ScenePlaythroughStatus.InProgress)
            .OrderBy(scene => scene.StartedAt)
            .ThenBy(scene => scene.Id)
            .ToList();

        return new PublicPlaythroughSnapshotDto
        {
            Name = playthrough.Name,
            ActiveSceneName = activeScenes.Count == 0
                ? null
                : string.Join(", ", activeScenes.Select(scene => scene.Name)),
            JourneyCharacters = playthrough.JourneyCharacters
                .OrderBy(character => character.SourceJourneyCharacterId)
                .ThenBy(character => character.Id)
                .Select(ToPublicCharacterDto)
                .ToList(),
            SceneCharacters = activeScenes
                .SelectMany(scene => scene.SceneCharacters)
                .Where(character => character.IsActive)
                .OrderBy(character => character.PlaythroughCharacter.CharacterType)
                .ThenBy(character => character.SourceSceneCharacterId)
                .ThenBy(character => character.Id)
                .Select(ToPublicCharacterDto)
                .ToList(),
            EventLogs = playthrough.EventLogs
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

    private static PublicPlaythroughCharacterDto ToPublicCharacterDto(
        JourneyPTCharacter character)
    {
        var equipmentEffects = ScenePlaythroughEquipmentEffects.For(character);
        var displayedCharacter = character.IsInAlternateForm && character.AlternateForm is not null
            ? character.AlternateForm
            : character.PlaythroughCharacter;

        return new PublicPlaythroughCharacterDto
        {
            Id = character.Id,
            IsSceneCharacter = false,
            CharacterType = displayedCharacter.CharacterType,
            Name = displayedCharacter.Name,
            Description = displayedCharacter.Description,
            PhotoUrl = displayedCharacter.PhotoUrl,
            PortraitUrl = displayedCharacter.PortraitUrl,
            CurrentHp = character.CurrentHp,
            MaxHp = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxHp, equipmentEffects.MaxHpModifier, minimum: 1),
            CurrentMp = character.CurrentMp,
            MaxMp = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxMp, equipmentEffects.MaxMpModifier),
            Movement = ScenePlaythroughEquipmentEffects.Apply(
                character.Movement, equipmentEffects.MovementModifier),
            MeleeAttackDamage = ScenePlaythroughEquipmentEffects.Apply(
                character.MeleeAttackDamage,
                equipmentEffects.MeleeAttackDamageModifier),
            BowAttackDamage = ScenePlaythroughEquipmentEffects.Apply(
                character.BowAttackDamage,
                equipmentEffects.BowAttackDamageModifier),
            MaxConsumableInventory = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxConsumableInventory,
                equipmentEffects.MaxConsumableInventoryModifier),
            MaxEquippableInventory = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxEquippableInventory,
                equipmentEffects.MaxEquippableInventoryModifier),
            IsActive = character.IsActive,
            IsDown = character.IsDown,
            IsDead = false,
            IsInAlternateForm = character.IsInAlternateForm,
            Spells = character.Spells
                .Select(link => link.PlaythroughSpell)
                .Concat(equipmentEffects.AddedSpells)
                .GroupBy(spell => spell.Id)
                .Select(group => group.First())
                .Select(spell => spell.ToPublicDto(
                    equipmentEffects.GetSpellDamageModifier(
                        spell.PlaythroughSpellTypeId)))
                .OrderBy(spell => spell.Name)
                .ToList(),
            ConsumableItems = character.ConsumableItems
                .Select(link => link.PlaythroughConsumableItem.ToPublicDto(link.IsUsed))
                .OrderBy(item => item.Name)
                .ToList(),
            EquippableItems = character.EquippableItems
                .Select(link => link.PlaythroughEquippableItem.ToPublicDto(true))
                .OrderByDescending(item => item.IsEquipped)
                .ThenBy(item => item.Name)
                .ToList()
        };
    }

    private static PublicPlaythroughCharacterDto ToPublicCharacterDto(
        ScenePTCharacter character)
    {
        var equipmentEffects = ScenePlaythroughEquipmentEffects.For(character);
        var displayedCharacter = character.IsInAlternateForm && character.AlternateForm is not null
            ? character.AlternateForm
            : character.PlaythroughCharacter;

        return new PublicPlaythroughCharacterDto
        {
            Id = character.Id,
            IsSceneCharacter = true,
            CharacterType = displayedCharacter.CharacterType,
            Name = displayedCharacter.Name,
            Description = displayedCharacter.Description,
            PhotoUrl = displayedCharacter.PhotoUrl,
            PortraitUrl = displayedCharacter.PortraitUrl,
            CurrentHp = character.CurrentHp,
            MaxHp = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxHp, equipmentEffects.MaxHpModifier, minimum: 1),
            CurrentMp = character.CurrentMp,
            MaxMp = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxMp, equipmentEffects.MaxMpModifier),
            Movement = ScenePlaythroughEquipmentEffects.Apply(
                character.Movement, equipmentEffects.MovementModifier),
            MeleeAttackDamage = ScenePlaythroughEquipmentEffects.Apply(
                character.MeleeAttackDamage,
                equipmentEffects.MeleeAttackDamageModifier),
            BowAttackDamage = ScenePlaythroughEquipmentEffects.Apply(
                character.BowAttackDamage,
                equipmentEffects.BowAttackDamageModifier),
            MaxConsumableInventory = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxConsumableInventory,
                equipmentEffects.MaxConsumableInventoryModifier),
            MaxEquippableInventory = ScenePlaythroughEquipmentEffects.Apply(
                character.MaxEquippableInventory,
                equipmentEffects.MaxEquippableInventoryModifier),
            IsActive = character.IsActive,
            IsDown = false,
            IsDead = character.IsDead,
            IsInAlternateForm = character.IsInAlternateForm,
            Spells = character.Spells
                .Select(link => link.PlaythroughSpell)
                .Concat(equipmentEffects.AddedSpells)
                .GroupBy(spell => spell.Id)
                .Select(group => group.First())
                .Select(spell => spell.ToPublicDto(
                    equipmentEffects.GetSpellDamageModifier(
                        spell.PlaythroughSpellTypeId)))
                .OrderBy(spell => spell.Name)
                .ToList(),
            ConsumableItems = character.ConsumableItems
                .Select(link => link.PlaythroughConsumableItem.ToPublicDto(link.IsUsed))
                .OrderBy(item => item.Name)
                .ToList(),
            EquippableItems = character.EquippableItems
                .Select(link => link.PlaythroughEquippableItem.ToPublicDto(true))
                .OrderByDescending(item => item.IsEquipped)
                .ThenBy(item => item.Name)
                .ToList()
        };
    }

    private static PublicPlaythroughSpellDto ToPublicDto(
        this PlaythroughSpell spell,
        int damageModifier = 0)
    {
        return new PublicPlaythroughSpellDto
        {
            Id = spell.Id,
            Name = spell.Name,
            Description = spell.Description,
            PhotoUrl = spell.PhotoUrl,
            SpellType = spell.PlaythroughSpellType.TypeName,
            Range = spell.Range,
            IsRadius = spell.IsRadius,
            MpCost = spell.MpCost,
            DamageEffect = spell.DamageEffect is int damageEffect
                ? ScenePlaythroughEquipmentEffects.Apply(
                    damageEffect, damageModifier)
                : null,
            HealthEffect = spell.HealthEffect,
            MagicEffect = spell.MagicEffect
        };
    }

    private static PublicPlaythroughConsumableItemDto ToPublicDto(
        this PlaythroughConsumableItem item,
        bool isUsed)
    {
        return new PublicPlaythroughConsumableItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            PhotoUrl = item.PhotoUrl,
            HpEffect = item.HpEffect,
            MpEffect = item.MpEffect,
            IsUsed = isUsed
        };
    }

    private static PublicPlaythroughEquippableItemDto ToPublicDto(
        this PlaythroughEquippableItem item,
        bool isEquipped)
    {
        return new PublicPlaythroughEquippableItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            PhotoUrl = item.PhotoUrl,
            IsEquipped = isEquipped,
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
                .Select(ToPublicDto)
                .OrderBy(spell => spell.Name)
                .ToList()
        };
    }
}
