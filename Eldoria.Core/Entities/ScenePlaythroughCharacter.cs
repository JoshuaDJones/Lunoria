namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughCharacter
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

        public bool IsDead { get; set; }

        public int ScenePlaythroughId { get; set; }
        public ScenePlaythrough ScenePlaythrough { get; set; } = null!;

        public int? AlternateFormId { get; set; }
        public ScenePlaythroughCharacter? AlternateForm { get; set; }
        public bool IsInAlternateForm { get; set; }

        public int SceneCharacterId { get; set; }
        public SceneCharacter SceneCharacter { get; set; } = null!;

        public ICollection<ScenePlaythroughCharacterSpell> CharacterSpells { get; set; } = [];
        public ICollection<ScenePlaythroughParticipant> SceneParticipants { get; set; } = [];
        public ICollection<ScenePlaythroughCharacterConsumableItem> ConsumableItems { get; set; } = [];
        public ICollection<ScenePlaythroughCharacterEquippableItem> EquippableItems { get; set; } = [];
    }
}
