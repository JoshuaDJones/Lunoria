using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterMappings
{
    public static ScenePlaythroughCharacterDto ToDto(this ScenePlaythroughCharacter character) => new()
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
        ScenePlaythrough = character.ScenePlaythrough.ToDto(),
        AlternateFormId = character.AlternateFormId,
        AlternateForm = character.AlternateForm?.ToDto(),
        IsInAlternateForm = character.IsInAlternateForm,
        SceneCharacterId = character.SceneCharacterId,
        SceneCharacter = character.SceneCharacter.ToDto(),
        CharacterSpells = [.. character.CharacterSpells.Select(spell => spell.ToDto())],
        SceneParticipants = [.. character.SceneParticipants.Select(participant => participant.ToDto())],
        ConsumableItems = [.. character.ConsumableItems.Select(item => item.ToDto())],
        EquippableItems = [.. character.EquippableItems.Select(item => item.ToDto())]
    };
}
