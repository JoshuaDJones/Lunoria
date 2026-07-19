using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughEvent
    {
        public int Id { get; set; }
        public SceneEventExecutionStatus ExecutionStatus { get; set; } = SceneEventExecutionStatus.NotStarted;
        public string? ErrorMessage { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int ScenePlaythroughId { get; set; }
        public ScenePlaythrough ScenePlaythrough { get; set; } = null!;

        public int SceneEventId { get; set; }
        public SceneEvent SceneEvent { get; set; } = null!;
    }
}
