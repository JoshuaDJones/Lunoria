using Eldoria.Core.Snapshots;

namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughDto
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int RevisionId { get; set; }
        public int RevisionNumber { get; set; }
        public int SnapshotSchemaVersion { get; set; }
        public JourneySnapshotV1 Snapshot { get; set; } = null!;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
