using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateSpellTypeRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = default!;

        [Required]
        public IFormFile Photo { get; set; } = default!;
    }
}
