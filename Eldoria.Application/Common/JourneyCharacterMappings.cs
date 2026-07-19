using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyCharacterMappings
    {
        public static JourneyCharacterDto ToDto(this JourneyCharacter journeyCharacter)
        {
            return new JourneyCharacterDto
            {
               Id = journeyCharacter.Id,
               MeleeAttackDamage = journeyCharacter.MeleeAttackDamage,
               BowAttackDamage = journeyCharacter.BowAttackDamage,
               Movement = journeyCharacter.Movement,
               MaxConsumableInventory = journeyCharacter.MaxConsumableInventory,
               MaxEquippableInventory = journeyCharacter.MaxEquippableInventory,
               MaxHp = journeyCharacter.MaxHp,
               MaxMp = journeyCharacter.MaxMp,
               IsInitiallyActive = journeyCharacter.IsInitiallyActive,
               Journey = journeyCharacter.Journey.ToDto(),
               AlternateForm = journeyCharacter.AlternateForm?.ToDto(),
               Character = journeyCharacter.Character.ToDto(),
               JourneyCharacterSpells = [.. journeyCharacter.JourneyCharacterSpells.Select(jcs => jcs.ToDto())]
            };
        }
    }
}