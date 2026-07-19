namespace Eldoria.Core.Entities
{
    public class Spell
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? FileName { get; set; }
        public int Range { get; set; }
        public bool IsRadius { get; set; }
        public int MpCost { get; set; }
        public int? DamageEffect { get; set; }
        public int? HealthEffect { get; set; }
        public int? MagicEffect { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int SpellTypeId { get; set; }
        public SpellType SpellType { get; set; } = null!;

        public ICollection<CharacterSpell> CharacterSpells { get; set; } = [];
        public ICollection<JourneyCharacterSpell> JourneyCharacterSpells { get; set; } = [];
        public ICollection<EquippableItem> EquippableItems { get; set; } = [];
    }
}
