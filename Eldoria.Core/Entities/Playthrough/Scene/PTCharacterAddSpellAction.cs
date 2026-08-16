using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class PTCharacterAddSpellAction
    {
        public int Id { get; set; }
        public int SourceCharacterAddSpellActionId { get; set; }

        public int? PlaythroughCharacterId { get; set; }
        public PlaythroughCharacter? PlaythroughCharacter { get; set; }

        public int PlaythroughSpellId { get; set; }
        public PlaythroughSpell PlaythroughSpell { get; set; } = null!;

        public int ScenePTActionEventId { get; set; }
        public ScenePTActionEvent ScenePTActionEvent { get; set; } = null!;
    }
}
