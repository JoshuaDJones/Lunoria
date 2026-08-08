using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateConsumableItemRequest
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(250)]
        public string Description { get; set; } = default!;

        [Required]
        public IFormFile Photo { get; set; } = default!;

        [Required]
        public int? HpEffect { get; set; }

        [Required]
        public int? MpEffect { get; set; }
    }
}
