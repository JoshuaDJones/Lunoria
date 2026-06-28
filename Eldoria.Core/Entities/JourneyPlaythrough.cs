namespace Eldoria.Core.Entities
{
    public class JourneyPlaythrough
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsActive { get; set; }

        public int JourneyId { get; set; }
        public Journey Journey { get; set; } = null!;

        public ICollection<SceneProgress> SceneProgressRecords { get; set; } = [];
    }
}
