namespace Eldoria.Core.Entities
{
    public class SceneEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public ICollection<SceneEventAction> SceneEventActions { get; set; } = [];
    }
}
