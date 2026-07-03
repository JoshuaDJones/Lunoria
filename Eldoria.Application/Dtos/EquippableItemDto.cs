namespace Eldoria.Application.Dtos
{
    internal class EquippableItemDto
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
        public ICollection<SpellDto> AddedSpells { get; set; } = [];

        public int MeleeDamageReduction { get; set; }
        public int BowDamageReduction { get; set; }
        public int SpellDamageReduction { get; set; }

        public int? AffectedSpellTypeId { get; set; }
        public SpellTypeDto? AffectedSpellType { get; set; }
        public int SpellDamageModifier { get; set; }
    }
}
