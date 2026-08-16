
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Scene;
using PlaythroughEntity = Eldoria.Core.Entities.Playthrough.Base.Playthrough;

namespace Eldoria.Core.Entities.Playthrough.Journey
{
    public class JourneyPTCharacter
    {
        public int Id { get; set; }
        public int SourceJourneyCharacterId { get; set; }

        public int? InitialMeleeAttackDamage { get; set; }
        public int? InitialBowAttackDamage { get; set; }
        public int InitialMovement { get; set; }
        public int InitialMaxConsumableInventory { get; set; }
        public int InitialMaxEquippableInventory { get; set; }
        public int InitialMaxHp { get; set; }
        public int InitialMaxMp { get; set; }
        public bool IsInitiallyActive { get; set; }

        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }
        public int Movement { get; set; }
        public int MaxConsumableInventory { get; set; }
        public int MaxEquippableInventory { get; set; }

        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }

        public bool IsActive { get; set; }
        public bool IsDown { get; set; }
        public bool IsInAlternateForm { get; set; }

        public int? AlternateFormId { get; set; }
        public PlaythroughCharacter? AlternateForm { get; set; }

        public int PlaythroughId { get; set; }
        public PlaythroughEntity Playthrough { get; set; } = null!;

        public int PlaythroughCharacterId { get; set; }
        public PlaythroughCharacter PlaythroughCharacter { get; set; } = null!;

        public ICollection<JourneyPTCharacterSpell> Spells { get; set; } = [];
        public ICollection<JourneyPTCharacterConsumableItem> ConsumableItems { get; set; } = [];
        public ICollection<JourneyPTCharacterEquippableItem> EquippableItems { get; set; } = [];
        public ICollection<ScenePTParticipant> SceneParticipants { get; set; } = [];
    }
}
