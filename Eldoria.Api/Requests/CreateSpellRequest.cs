using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateSpellRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        [Required]
        public IFormFile Photo { get; set; } = default!;

        [Required]
        public int? Range { get; set; }

        [Required]
        public bool? IsRadius { get; set; }

        [Required]
        public int? MpCost { get; set; }

        public int? DamageEffect { get; set; }
        public int? HealthEffect { get; set; }
        public int? MagicEffect { get; set; }
    }
}
