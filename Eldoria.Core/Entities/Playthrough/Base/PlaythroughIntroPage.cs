using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class PlaythroughIntroPage
    {
        public int Id { get; set; }
        public int SourceIntroPageId { get; set; }
        public int SortOrder { get; set; }
        public IntroPageType Type { get; set; }
        public string Config { get; set; } = "{}";
        public string? PreviewPhotoUrl { get; set; }

        public int PlaythroughId { get; set; }
        public Playthrough Playthrough { get; set; } = null!;
    }
}
