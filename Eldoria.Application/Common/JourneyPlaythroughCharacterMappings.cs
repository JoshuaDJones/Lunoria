using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Application.Common
{
    public static class JourneyPlaythroughCharacterMappings
    {
        public static JourneyPlaythroughCharacterDto ToDto(
            this PlaythroughCharacter character)
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
                JourneyPlaythroughId = character.JourneyPlaythroughId,
                AlternateFormId = character.AlternateFormId,
                IsInAlternateForm = character.IsInAlternateForm,
                SourceJourneyCharacterId = character.JourneyCharacterId,
                SnapshotCharacterKey = character.SnapshotCharacterKey,
                SnapshotAssignmentKey = character.SnapshotAssignmentKey,
                CharacterSpells = [.. character.CharacterSpells.Select(spell => spell.ToDto())],
                ConsumableItems = [.. character.ConsumableItems.Select(item => item.ToDto())],
                EquippableItems = [.. character.EquippableItems.Select(item => item.ToDto())]
            };
        }
    }
}
