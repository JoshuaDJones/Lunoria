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
                MaxHp = character.MaxHp,
                MaxMp = character.MaxMp,
                MeleeAttackDamage = character.MeleeAttackDamage,
                BowAttackDamage = character.BowAttackDamage,
                Movement = character.Movement,
                MaxInventory = character.MaxInventory,
                IsPlayer = character.IsPlayer,
                IsNPC = character.IsNPC,
                IsEnemy = character.IsEnemy,
                CreateDate = character.CreateDate,
                CharacterSpells = character.CharacterSpells.Select(s => s.ToDto()).ToList()
            };
        }
    }
}
