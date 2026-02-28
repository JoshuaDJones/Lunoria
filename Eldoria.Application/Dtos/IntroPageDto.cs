using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class IntroPageDto
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int Order { get; set; }
        public IntroPageType Type { get; set; }
        public string Config { get; set; } = "{}";
    }
}
