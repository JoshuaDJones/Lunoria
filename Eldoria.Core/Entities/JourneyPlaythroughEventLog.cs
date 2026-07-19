namespace Eldoria.Core.Entities
{
    public class JourneyPlaythroughEventLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }

        public int JourneyPlaythroughId { get; set; }
        public JourneyPlaythrough JourneyPlaythrough { get; set; } = null!;
    }
}
