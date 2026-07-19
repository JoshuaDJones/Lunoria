using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class CharacterStatAdjustmentAction
    {
        public int Id { get; set; }
        public CharacterStatType CharacterStatType { get; set; }
        public AdjustmentOperation AdjustmentOperation { get; set; }
        public int Value { get; set; }

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        public int SceneEventActionId { get; set; }
        public SceneEventAction SceneEventAction { get; set; } = null!;
    }
}
