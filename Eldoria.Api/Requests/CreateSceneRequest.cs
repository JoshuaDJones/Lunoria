using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateSceneRequest
    {
        [Required]
        public int? JourneyId { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        [Required]
        public IFormFile Photo { get; set; } = default!;

        [Required]
        public string GridUrl { get; set; } = default!;
    }
}
