namespace Eldoria.Core.Entities
{
    public class JourneyRevision
    {
        public int Id { get; set; }
        public int RevisionNumber { get; set; }
        public int SchemaVersion { get; init; }
        public string ContentHash { get; init; } = string.Empty;
        public string SnapshotJson { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }

        public int? SourceJourneyId { get; init; }
        public Journey? SourceJourney { get; set; }

        public int CreatedByUserId { get; init; }
        public User CreatedByUser { get; set; } = null!;

        public ICollection<JourneyPlaythrough> Playthroughs { get; set; } = [];
    }
}
