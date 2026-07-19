namespace Eldoria.Core.Entities
{
    public class JourneyPlaythroughCharacter
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
        public JourneyPlaythrough JourneyPlaythrough { get; set; } = null!;

        public int? AlternateFormId { get; set; }
        public JourneyPlaythroughCharacter? AlternateForm { get; set; }
        public bool IsInAlternateForm { get; set; }

        public int JourneyCharacterId { get; set; }
        public JourneyCharacter JourneyCharacter { get; set; } = null!;

        public ICollection<JourneyPlaythroughCharacterSpell> CharacterSpells { get; set; } = [];
        public ICollection<ScenePlaythroughParticipant> SceneParticipants { get; set; } = [];
        public ICollection<JourneyPlaythroughCharacterConsumableItem> ConsumableItems { get; set; } = [];
        public ICollection<JourneyPlaythroughCharacterEquippableItem> EquippableItems { get; set; } = [];
    }
}
