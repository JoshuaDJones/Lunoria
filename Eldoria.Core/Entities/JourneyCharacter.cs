namespace Eldoria.Core.Entities
{
    public class JourneyCharacter
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

        public int JourneyId { get; set; }
        public Journey Journey { get; set; } = null!;

        public int? AlternateFormId { get; set; }
        public Character? AlternateForm { get; set; } = null!;
        public bool IsInAlternateForm { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public ICollection<JourneyCharacterItem> JourneyCharacterItems { get; set; } = [];
        public ICollection<JourneyCharacterEquippableItem> JourneyCharacterEquippableItems { get; set; } = [];
        public ICollection<JourneyCharacterSpell> JourneyCharacterSpells { get; set; } = [];
        public ICollection<SceneParticipant> SceneParticipants { get; set; } = [];
    }
}
