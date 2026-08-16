using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterMappings
{
    public static ScenePlaythroughCharacterDto ToDto(this ScenePTCharacter character) => new()
    {
        Id = character.Id,
        MeleeAttackDamage = character.MeleeAttackDamage,
        BowAttackDamage = character.BowAttackDamage,
        Movement = character.Movement,
        MaxConsumableInventory = character.MaxConsumableInventory,
        MaxEquippableInventory = character.MaxEquippableInventory,
        CurrentHp = character.CurrentHp,
        CurrentMp = character.CurrentMp,
        MaxHp = character.MaxHp,
        MaxMp = character.MaxMp,
        IsDead = character.IsDead,
        ScenePlaythroughId = character.ScenePlaythroughId,
        AlternateFormId = character.AlternateFormId,
        IsInAlternateForm = character.IsInAlternateForm,
        SourceSceneCharacterId = character.SceneCharacterId,
        SnapshotCharacterKey = character.SnapshotCharacterKey,
        SnapshotAssignmentKey = character.SnapshotAssignmentKey,
        CharacterSpells = [.. character.CharacterSpells.Select(spell => spell.ToDto())],
        ConsumableItems = [.. character.ConsumableItems.Select(item => item.ToDto())],
        EquippableItems = [.. character.EquippableItems.Select(item => item.ToDto())]
    };
}
