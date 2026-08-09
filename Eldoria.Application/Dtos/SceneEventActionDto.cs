using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class SceneEventActionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public ActionTargetType ActionTargetType { get; set; }
        public EventActionType EventActionType { get; set; }
        public int SceneEventId { get; set; }
        public CharacterStatAdjustmentActionDto? CharacterStatAdjustmentAction { get; set; }
    }
}
