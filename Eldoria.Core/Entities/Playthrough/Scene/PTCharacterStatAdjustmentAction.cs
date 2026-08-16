using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class PTCharacterStatAdjustmentAction
    {
        public int Id { get; set; }
        public int SourceCharacterStatAdjustmentActionId { get; set; }
        public CharacterStatType CharacterStatType { get; set; }
        public AdjustmentOperation AdjustmentOperation { get; set; }
        public int Value { get; set; }

        public int? PlaythroughCharacterId { get; set; }
        public PlaythroughCharacter? Character { get; set; }

        public int ScenePTActionEventId { get; set; }
        public ScenePTActionEvent ScenePTActionEvent { get; set; } = null!;
    }
}
