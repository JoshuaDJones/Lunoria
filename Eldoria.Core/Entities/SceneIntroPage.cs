using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class SceneIntroPage
    {
        public int Id { get; set; }        
        public int SortOrder { get; set; }
        public IntroPageType Type { get; set; }
        public string Config { get; set; } = "{}";
        public string? PreviewPhotoUrl { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;
    }
}
