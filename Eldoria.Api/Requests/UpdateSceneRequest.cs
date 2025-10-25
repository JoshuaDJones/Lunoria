using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateSceneRequest
    {
        [Required]
        public int? JourneyId { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        public IFormFile? Photo { get; set; }

        [Required]
        public string GridUrl { get; set; } = default!;
    }
}
