using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTCharacterSpell
    {
        public int Id { get; set; }
        public int? SourceSceneCharacterSpellId { get; set; }

        public int ScenePTCharacterId { get; set; }
        public ScenePTCharacter ScenePTCharacter { get; set; } = null!;

        public int PlaythroughSpellId { get; set; }
        public PlaythroughSpell PlaythroughSpell { get; set; } = null!;
    }
}
