namespace Eldoria.Core.Entities
{
    public class JourneyPlaythrough
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsActive { get; set; }
        public int SourceJourneyId { get; set; }

        public int? JourneyId { get; set; }
        public Journey? Journey { get; set; }

        public int JourneyRevisionId { get; set; }
        public JourneyRevision JourneyRevision { get; set; } = null!;

        public ICollection<JourneyPlaythroughCharacter> JourneyCharacters { get; set; } = [];
        public ICollection<ScenePlaythrough> ScenePlaythroughs { get; set; } = [];
        public ICollection<JourneyPlaythroughEventLog> PlaythroughEventLogs { get; set; } = [];
    }
}
