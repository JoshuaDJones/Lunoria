namespace Eldoria.Application.Dtos
{
    public class JourneyCharacterDto
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int CharacterId { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }
        public int Movement { get; set; }
        public int MaxConsumableInventory { get; set; }
        public int MaxEquippableInventory { get; set; }
        public bool IsDown { get; set; }
        public int? AlternateFormId { get; set; }
        public bool IsAlternateForm { get; set; }
        public CharacterDto? AlternateForm { get; set; }
        public CharacterDto Character { get; set; } = null!;
        public List<JourneyCharacterItemDto> JourneyCharacterItems { get; set; } = [];
        public List<JourneyCharacterEquippableItemDto> JourneyCharacterEquippableItems { get; set; } = [];
        public List<JourneyCharacterSpellDto> JourneyCharacterSpells { get; set; } = [];

        public int EffectiveMaxHp { get; set; }
        public int EffectiveMaxMp { get; set; }
        public int? EffectiveMeleeAttackDamage { get; set; }
        public int? EffectiveBowAttackDamage { get; set; }
        public int EffectiveMovement { get; set; }
        public int EffectiveMaxConsumableInventory { get; set; }
        public int EffectiveMaxEquippableInventory { get; set; }
        public int EffectiveMeleeDamageReduction { get; set; }
        public int EffectiveBowDamageReduction { get; set; }
        public int EffectiveSpellDamageReduction { get; set; }
        public List<SpellDamageModifierDto> EffectiveSpellDamageModifiers { get; set; } = [];
        public List<SpellDto> EffectiveSpells { get; set; } = [];
    }
}
