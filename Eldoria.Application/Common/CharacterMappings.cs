using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class CharacterMappings
    {
        public static CharacterDto ToDto(this Character character)
        {
            return new CharacterDto
            {
                Id = character.Id,
                Name = character.Name,
                Description = character.Description,
                PhotoUrl = character.PhotoUrl,
                MaxHp = character.BaseMaxHp,
                MaxMp = character.BaseMaxMp,
                MeleeAttackDamage = character.BaseMeleeAttackDamage,
                BowAttackDamage = character.BaseBowAttackDamage,
                Movement = character.BaseMovement,
                MaxInventory = character.BaseMaxConsumableInventory,
                IsPlayer = character.IsPlayer,
                IsNPC = character.IsNPC,
                IsEnemy = character.IsEnemy,
                AlternateForm = character.BaseAlternateForm?.ToDto(),
                CreateDate = character.CreateDate,
                CharacterSpells = character.CharacterSpells.Select(s => s.ToDto()).ToList(),
                CharacterDialogSettings = character.CharacterDialogSettings == null ? null : character.CharacterDialogSettings.ToDto()
            };
        }
    }
}
