namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughEventLogDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }
        public JourneyPlaythroughDto JourneyPlaythrough { get; set; } = null!;
    }
}
