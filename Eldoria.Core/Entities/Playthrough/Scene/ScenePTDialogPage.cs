using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTDialogPage
    {

        public int Id { get; set; }
        public int SourceDialogPageId { get; set; }
        public int OrderNum { get; set; }
        public DialogPageType PageType { get; set; } = DialogPageType.Image;
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaBlobName { get; set; } = string.Empty;
        public string MediaContentType { get; set; } = string.Empty;

        public int SceneDialogId { get; set; }
        public ScenePTDialog SceneDialog { get; set; } = null!;

        public ICollection<ScenePTDialogSection> DialogPageSections { get; set; } = [];
    }
}
