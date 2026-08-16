namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class PlaythroughEventLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }

        public int PlaythroughId { get; set; }
        public Playthrough Playthrough { get; set; } = null!;
    }
}
