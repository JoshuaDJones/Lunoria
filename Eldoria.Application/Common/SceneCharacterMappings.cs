using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SceneCharacterMappings
    {
        public static SceneCharacterDto ToDto(this SceneCharacter sceneCharacter)
        {
            return new SceneCharacterDto
            {
                Id = sceneCharacter.Id,
                MeleeAttackDamage = sceneCharacter.MeleeAttackDamage,
                BowAttackDamage = sceneCharacter.BowAttackDamage,
                Movement = sceneCharacter.Movement,
                MaxConsumableInventory = sceneCharacter.MaxConsumableInventory,
                MaxEquippableInventory = sceneCharacter.MaxEquippableInventory,
                MaxHp = sceneCharacter.MaxHp,
                MaxMp = sceneCharacter.MaxMp,
                IsInitiallyActive = sceneCharacter.IsInitiallyActive,
                SceneId = sceneCharacter.SceneId,
                AlternateForm = sceneCharacter.AlternateForm?.ToDto(),
                CharacterId = sceneCharacter.CharacterId,
                Character = sceneCharacter.Character.ToDto(),
                SceneCharacterSpells = [.. sceneCharacter.SceneCharacterSpells.Select(spell => spell.ToDto())]
            };
        }
    }
}
