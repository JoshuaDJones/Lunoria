using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateJourneyRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}
