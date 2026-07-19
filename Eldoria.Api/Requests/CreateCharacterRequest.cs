using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateCharacterRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public IFormFile Photo { get; set; } = null!;

        [Required]
        public int? MaxHp { get; set; }

        [Required]
        public int? MaxMp { get; set; }

        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }

        [Required]
        public int? Movement { get; set; }

        [Required]
        public int? BaseMaxConsumableInventory { get; set; }

        [Required]
        public int? BaseMaxEquippableInventory { get; set; }

        [Required]
        public Eldoria.Core.Enums.CharacterType? CharacterType { get; set; }

        public int? AlternateFormId { get; set; }
    }
}
