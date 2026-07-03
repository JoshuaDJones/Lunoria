using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Models
{
    public class EquippableItemInput
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
        public int MeleeAttackDamageModifier { get; set; }
        public int BowAttackDamageModifier { get; set; }
        public int MovementModifier { get; set; }
        public int MaxHpModifier { get; set; }
        public int MaxMpModifier { get; set; }
        public int MaxConsumableInventoryModifier { get; set; }
        public int MaxEquippableInventoryModifier { get; set; }
        public int MeleeDamageReduction { get; set; }
        public int BowDamageReduction { get; set; }
        public int SpellDamageReduction { get; set; }
        public int? AffectedSpellTypeId { get; set; }
        public int SpellDamageModifier { get; set; }
        public List<int> AddedSpellIds { get; set; } = [];
    }
}
