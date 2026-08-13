namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughCharacterDto
    {
        public int Id { get; set; }

        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }
        public int Movement { get; set; }
        public int MaxConsumableInventory { get; set; }
        public int MaxEquippableInventory { get; set; }

        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }

        public bool IsDown { get; set; }

        public int JourneyPlaythroughId { get; set; }
        public int? AlternateFormId { get; set; }
        public bool IsInAlternateForm { get; set; }

        public int? SourceJourneyCharacterId { get; set; }
        public string SnapshotCharacterKey { get; set; } = string.Empty;
        public string SnapshotAssignmentKey { get; set; } = string.Empty;

        public List<JourneyPlaythroughCharacterSpellDto> CharacterSpells { get; set; } = [];
        public List<JourneyPlaythroughCharacterConsumableItemDto> ConsumableItems { get; set; } = [];
        public List<JourneyPlaythroughCharacterEquippableItemDto> EquippableItems { get; set; } = [];
    }
}
