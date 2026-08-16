using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTActionEvent
    {
        public int Id { get; set; }
        public int SourceSceneEventActionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public ActionTargetType ActionTargetType { get; set; }
        public EventActionType EventActionType { get; set; }

        public int ScenePTEventId { get; set; }
        public ScenePTEvent SceneEvent { get; set; } = null!;

        public PTCharacterStatAdjustmentAction? CharacterStatAdjustmentAction { get; set; }
        public PTCharacterAddSpellAction? CharacterAddSpellAction { get; set; }
    }
}
