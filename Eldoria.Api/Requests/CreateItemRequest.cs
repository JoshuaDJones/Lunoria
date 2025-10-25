using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateItemRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        [Required]
        public IFormFile Photo { get; set; } = default!;

        [Required]
        public int? HpEffect { get; set; }

        [Required]
        public int? MpEffect { get; set; }
    }
}
