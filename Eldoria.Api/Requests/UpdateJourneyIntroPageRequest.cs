using System.ComponentModel.DataAnnotations;
using Eldoria.Core.Enums;

namespace Eldoria.Api.Requests
{
    public class UpdateJourneyIntroPageRequest
    {
        [Required]
        public int? JourneyId { get; set; }

        [Required]
        public IntroPageType? Type { get; set; }

        [Required]
        public string Config { get; set; } = default!;

        public IFormFile? Image { get; set; }
    }
}
