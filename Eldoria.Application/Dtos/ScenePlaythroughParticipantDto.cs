using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughParticipantDto
{
    public int Id { get; set; }
    public ParticipantType ParticipantType { get; set; }
    public int? SortOrderWithinType { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrentParticipant { get; set; }
    public int AttacksPerTurn { get; set; }
    public int AttacksRemaining { get; set; }
    public int? JourneyPlaythroughCharacterId { get; set; }
    public int? ScenePlaythroughCharacterId { get; set; }
    public int PlaythroughCharacterId { get; set; }
    public int DisplayedPlaythroughCharacterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? PortraitUrl { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMp { get; set; }
    public int MaxMp { get; set; }
    public int Movement { get; set; }
    public int? MeleeAttackDamage { get; set; }
    public int? BowAttackDamage { get; set; }
    public int MeleeDamageReduction { get; set; }
    public int BowDamageReduction { get; set; }
    public int SpellDamageReduction { get; set; }
    public bool IsDown { get; set; }
    public bool IsDead { get; set; }
    public bool IsInAlternateForm { get; set; }
    public bool CanTransform { get; set; }
    public int? DownedTurnsRemaining { get; set; }
    public int MaxConsumableInventory { get; set; }
    public int MaxEquippableInventory { get; set; }
    public List<ScenePlaythroughSpellDto> Spells { get; set; } = [];
    public List<ScenePlaythroughInventoryItemDto> ConsumableItems { get; set; } = [];
    public List<ScenePlaythroughInventoryItemDto> EquippableItems { get; set; } = [];
}
