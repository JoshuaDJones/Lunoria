using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class PTCharacterInAlternateFormAction
    {
        public int Id { get; set; }

        public int? PlaythroughCharacterId { get; set; }
        public PlaythroughCharacter? PlaythroughCharacter { get; set; }

        public int ScenePTActionEventId { get; set; }
        public ScenePTActionEvent ScenePTActionEvent { get; set; } = null!;
    }
}
