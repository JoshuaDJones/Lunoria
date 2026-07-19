namespace Eldoria.Core.Entities
{
    public class SceneDialog
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public ICollection<DialogPage> DialogPages { get; set; } = [];
    }
}
