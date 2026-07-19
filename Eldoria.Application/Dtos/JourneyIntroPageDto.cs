using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class JourneyIntroPageDto
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public IntroPageType Type { get; set; }
        public string Config { get; set; } = "{}";
        public string? PreviewPhotoUrl { get; set; }
        public JourneyDto Journey { get; set; } = null!;
    }
}
