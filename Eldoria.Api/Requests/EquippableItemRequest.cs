using Eldoria.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public abstract class EquippableItemRequest
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(250)]
        public string Description { get; set; } = default!;

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

        public EquippableItemInput ToInput(IFormFile? photo)
        {
            return new EquippableItemInput
            {
                Name = Name,
                Description = Description,
                Photo = photo,
                MeleeAttackDamageModifier = MeleeAttackDamageModifier,
                BowAttackDamageModifier = BowAttackDamageModifier,
                MovementModifier = MovementModifier,
                MaxHpModifier = MaxHpModifier,
                MaxMpModifier = MaxMpModifier,
                MaxConsumableInventoryModifier = MaxConsumableInventoryModifier,
                MaxEquippableInventoryModifier = MaxEquippableInventoryModifier,
                MeleeDamageReduction = MeleeDamageReduction,
                BowDamageReduction = BowDamageReduction,
                SpellDamageReduction = SpellDamageReduction,
                AffectedSpellTypeId = AffectedSpellTypeId,
                SpellDamageModifier = SpellDamageModifier,
                AddedSpellIds = AddedSpellIds,
            };
        }
    }
}
