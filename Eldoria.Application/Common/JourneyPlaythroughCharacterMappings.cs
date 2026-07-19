using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyPlaythroughCharacterMappings
    {
        public static JourneyPlaythroughCharacterDto ToDto(
            this JourneyPlaythroughCharacter character)
        {
            return new JourneyPlaythroughCharacterDto
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
                IsDown = character.IsDown,
                JourneyPlaythrough = character.JourneyPlaythrough.ToDto(),
                AlternateForm = character.AlternateForm?.ToDto(),
                IsInAlternateForm = character.IsInAlternateForm,
                JourneyCharacterId = character.JourneyCharacterId,
                JourneyCharacter = character.JourneyCharacter.ToDto(),
                CharacterSpells = [.. character.CharacterSpells.Select(spell => spell.ToDto())],
                SceneParticipants = [.. character.SceneParticipants.Select(participant => participant.ToDto())],
                ConsumableItems = [.. character.ConsumableItems.Select(item => item.ToDto())],
                EquippableItems = [.. character.EquippableItems.Select(item => item.ToDto())]
            };
        }
    }
}