namespace Eldoria.Core.Entities
{
    public class SceneGrid
    {
        public int Id { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string GridColor { get; set; } = "#ffffff";
        public string? BackgroundImageUrl { get; set; }
        public string? BackgroundFileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;
    }
}
