namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughDto
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
