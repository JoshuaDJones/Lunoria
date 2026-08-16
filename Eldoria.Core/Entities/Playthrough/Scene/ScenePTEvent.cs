using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTEvent
    {
        public int Id { get; set; }
        public int SourceSceneEventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }

        public SceneEventExecutionStatus ExecutionStatus { get; set; } = SceneEventExecutionStatus.NotStarted;
        public string? ErrorMessage { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int ScenePTId { get; set; }
        public ScenePT ScenePT { get; set; } = null!;

        public ICollection<ScenePTActionEvent> ScenePTActionEvents { get; set; } = [];
    }
}
