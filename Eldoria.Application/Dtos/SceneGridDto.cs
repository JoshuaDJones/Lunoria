namespace Eldoria.Application.Dtos
{
    public class SceneGridDto
    {
        public int Id { get; set; }
        public int SceneId { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string GridColor { get; set; } = string.Empty;
        public string? BackgroundImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
