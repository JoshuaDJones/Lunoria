using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateCharacterRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        public IFormFile? Photo { get; set; }

        [Required]
        public int? MaxHp { get; set; }

        [Required]
        public int? MaxMp { get; set; }


        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }

        [Required]
        public int? Movement { get; set; }

        [Required]
        public int? MaxInventory { get; set; }

        [Required]
        public bool? IsPlayer { get; set; }

        [Required]
        public bool? IsNPC { get; set; }

        [Required]
        public bool? IsEnemy { get; set; }
    }
}
