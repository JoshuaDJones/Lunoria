using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughEventDto
    {
        public int Id { get; set; }
        public SceneEventExecutionStatus ExecutionStatus { get; set; } = SceneEventExecutionStatus.NotStarted;
        public string? ErrorMessage { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int ScenePlaythroughId { get; set; }
        public int? SourceSceneEventId { get; set; }
        public string SnapshotEventKey { get; set; } = string.Empty;
    }
}
