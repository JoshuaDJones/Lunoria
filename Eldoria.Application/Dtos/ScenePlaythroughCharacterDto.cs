using Eldoria.Core.Entities;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughCharacterDto
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

        public ScenePlaythroughDto ScenePlaythrough { get; set; } = null!;

        public int? AlternateFormId { get; set; }
        public ScenePlaythroughCharacterDto? AlternateForm { get; set; }
        public bool IsInAlternateForm { get; set; }

        public int SceneCharacterId { get; set; }
        public SceneCharacterDto SceneCharacter { get; set; } = null!;

        public List<ScenePlaythroughCharacterSpellDto> CharacterSpells { get; set; } = [];
        public List<ScenePlaythroughParticipantDto> SceneParticipants { get; set; } = [];
        public List<ScenePlaythroughCharacterConsumableItemDto> ConsumableItems { get; set; } = [];
        public List<ScenePlaythroughCharacterEquippableItemDto> EquippableItems { get; set; } = [];
    }
}
