using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateSpellRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;
        public IFormFile? Photo { get; set; }

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
