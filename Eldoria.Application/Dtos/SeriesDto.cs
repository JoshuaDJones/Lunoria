namespace Eldoria.Application.Dtos
{
    public class SeriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public string? FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<JourneyDto> Journeys { get; set; } = [];
    }
}
