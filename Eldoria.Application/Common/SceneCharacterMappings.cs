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
                CurrentHp = sceneCharacter.CurrentHp,
                CurrentMp = sceneCharacter.CurrentMp,
                IsDown = sceneCharacter.IsDown,
                IsAlternateForm = sceneCharacter.IsAlternateForm,
                SceneId = sceneCharacter.SceneId,
                CharacterId = sceneCharacter.CharacterId,
                Character = sceneCharacter.Character.ToDto(),
                SceneCharacterItems = sceneCharacter.SceneCharacterItems.Select(sci => sci.ToDto()).ToList()
            };
        }
    }
}
