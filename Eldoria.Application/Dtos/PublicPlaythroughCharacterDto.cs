using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public sealed class PublicPlaythroughCharacterDto
{
    public int Id { get; set; }
    public bool IsSceneCharacter { get; set; }
    public CharacterType CharacterType { get; set; }
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
    public int MaxConsumableInventory { get; set; }
    public int MaxEquippableInventory { get; set; }
    public bool IsActive { get; set; }
    public bool IsDown { get; set; }
    public bool IsDead { get; set; }
    public bool IsInAlternateForm { get; set; }
    public PublicAlternateFormDto? AlternateForm { get; set; }
    public List<PublicPlaythroughSpellDto> Spells { get; set; } = [];
    public List<PublicPlaythroughConsumableItemDto> ConsumableItems { get; set; } = [];
    public List<PublicPlaythroughEquippableItemDto> EquippableItems { get; set; } = [];
}

public sealed class PublicAlternateFormDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public int Movement { get; set; }
    public int? MeleeAttackDamage { get; set; }
    public int? BowAttackDamage { get; set; }
    public int MaxConsumableInventory { get; set; }
    public int MaxEquippableInventory { get; set; }
    public List<PublicPlaythroughSpellDto> Spells { get; set; } = [];
}
