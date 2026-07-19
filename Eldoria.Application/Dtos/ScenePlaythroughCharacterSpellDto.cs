using Eldoria.Core.Entities;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughCharacterSpellDto
    {
        public int Id { get; set; }
        public ScenePlaythroughCharacterDto ScenePlaythroughCharacter { get; set; } = null!;
        public SceneCharacterSpellDto SceneCharacterSpell { get; set; } = null!;
    }
}
