namespace Eldoria.Core.Entities
{
    public class EquippableItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public int MeleeAttackDamageModifier { get; set; }
        public int BowAttackDamageModifier { get; set; }
        public int MovementModifier { get; set; }
        public int MaxHpModifier { get; set; }
        public int MaxMpModifier { get; set; }
        public int MaxConsumableInventoryModifier { get; set; }
        public int MaxEquippableInventoryModifier { get; set; }
        public ICollection<Spell> AddedSpells { get; set; } = [];

        public int MeleeDamageReduction { get; set; }
        public int BowDamageReduction { get; set; }
        public int SpellDamageReduction { get; set; }

        public int? AffectedSpellTypeId { get; set; }
        public SpellType? AffectedSpellType { get; set; }
        public int SpellDamageModifier { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<JourneyCharacterEquippableItem> JourneyCharacterEquippableItems { get; set; } = [];
    }
}
