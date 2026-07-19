using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class SceneEventAction
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public ActionTargetType ActionTargetType { get; set; }
        public EventActionType EventActionType { get; set; }

        public int SceneEventId { get; set; }
        public SceneEvent SceneEvent { get; set; } = null!;

        public CharacterStatAdjustmentAction? CharacterStatAdjustmentAction { get; set; }
    }
}
