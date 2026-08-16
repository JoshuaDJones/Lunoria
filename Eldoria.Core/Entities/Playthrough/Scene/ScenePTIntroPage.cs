using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTIntroPage
    {
        public int Id { get; set; }
        public int SourceIntroPageId { get; set; }
        public int SortOrder { get; set; }
        public IntroPageType Type { get; set; }
        public string Config { get; set; } = "{}";
        public string? PreviewPhotoUrl { get; set; }

        public int ScenePTId { get; set; }
        public ScenePT ScenePT { get; set; } = null!;
    }
}
