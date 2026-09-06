using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class ScenePTDialogPage
    {

        public int Id { get; set; }
        public int OrderNum { get; set; }
        public DialogPageType PageType { get; set; } = DialogPageType.Image;
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaBlobName { get; set; } = string.Empty;
        public string MediaContentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int SceneDialogId { get; set; }
        public SceneDialog SceneDialog { get; set; } = null!;

        public ICollection<ScenePTDialogSection> DialogPageSections { get; set; } = [];
    }
}
