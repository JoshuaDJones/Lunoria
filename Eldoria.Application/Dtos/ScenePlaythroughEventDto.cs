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
        public ScenePlaythroughDto ScenePlaythrough { get; set; } = null!;
        public SceneEventDto SceneEvent { get; set; } = null!;
    }
}
