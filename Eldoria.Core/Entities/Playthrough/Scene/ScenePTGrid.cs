namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTGrid
    {
        public int Id { get; set; }
        public int SourceSceneGridId { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string GridColor { get; set; } = "#ffffff";
        public string? BackgroundImageUrl { get; set; }
        public string? BackgroundFileName { get; set; }

        public int ScenePTId { get; set; }
        public ScenePT ScenePT { get; set; } = null!;
    }
}
