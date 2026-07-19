namespace Eldoria.Core.Entities
{
    public class DialogPage
    {

        public int Id { get; set; }
        public int OrderNum { get; set; }
        public string? PhotoUrl { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int SceneDialogId { get; set; }
        public SceneDialog SceneDialog { get; set; } = null!;

        public ICollection<DialogPageSection> DialogPageSections { get; set; } = [];
    }
}
