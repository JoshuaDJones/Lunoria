using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class IntroPage
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int Order { get; set; }
        public IntroPageType Type { get; set; }
        public string Config { get; set; } = "{}";
        public string? PreviewPhotoUrl { get; set; }
        public Journey Journey { get; set; } = null!;
    }
}
