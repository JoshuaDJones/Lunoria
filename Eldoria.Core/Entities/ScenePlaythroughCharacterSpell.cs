namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughCharacterSpell
    {
        public int Id { get; set; }

        public int ScenePlaythroughCharacterId { get; set; }
        public ScenePlaythroughCharacter ScenePlaythroughCharacter { get; set; } = null!;

        public int SceneCharacterSpellId { get; set; }
        public SceneCharacterSpell SceneCharacterSpell { get; set; } = null!;
    }
}
