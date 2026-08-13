using Eldoria.Core.Enums;

namespace Eldoria.Core.Snapshots;

public sealed class JourneySnapshotV1
{
    public const int Version = 1;
    public int SchemaVersion { get; init; } = Version;
    public required JourneyDefinitionSnapshot Journey { get; init; }
    public List<CharacterDefinitionSnapshot> Characters { get; init; } = [];
    public List<SpellTypeDefinitionSnapshot> SpellTypes { get; init; } = [];
    public List<SpellDefinitionSnapshot> Spells { get; init; } = [];
    public List<ConsumableDefinitionSnapshot> Consumables { get; init; } = [];
    public List<EquipmentDefinitionSnapshot> Equipment { get; init; } = [];
    public List<SceneDefinitionSnapshot> Scenes { get; init; } = [];
}

public sealed class JourneyDefinitionSnapshot
{
    public int SourceJourneyId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string PhotoUrl { get; init; }
    public required string FileName { get; init; }
    public int SortOrder { get; init; }
    public List<IntroPageDefinitionSnapshot> IntroPages { get; init; } = [];
    public List<JourneyCharacterDefinitionSnapshot> Characters { get; init; } = [];
    public List<string> SceneKeys { get; init; } = [];
}

public sealed class IntroPageDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceId { get; init; }
    public int SortOrder { get; init; }
    public IntroPageType Type { get; init; }
    public required string Config { get; init; }
    public string? PreviewPhotoUrl { get; init; }
}

public sealed class CharacterDialogDefinitionSnapshot
{
    public required string ActiveColor { get; init; }
    public required string InactiveColor { get; init; }
}

public sealed class CharacterDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceCharacterId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string PhotoUrl { get; init; }
    public required string FileName { get; init; }
    public string? PortraitUrl { get; init; }
    public string? PortraitFileName { get; init; }
    public int BaseMaxHp { get; init; }
    public int BaseMaxMp { get; init; }
    public int? BaseMeleeAttackDamage { get; init; }
    public int? BaseBowAttackDamage { get; init; }
    public int BaseMovement { get; init; }
    public int BaseMaxConsumableInventory { get; init; }
    public int BaseMaxEquippableInventory { get; init; }
    public CharacterType CharacterType { get; init; }
    public string? BaseAlternateFormCharacterKey { get; init; }
    public CharacterDialogDefinitionSnapshot? DialogSettings { get; init; }
    public List<string> SpellKeys { get; init; } = [];
}

public sealed class JourneyCharacterDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceJourneyCharacterId { get; init; }
    public required string CharacterKey { get; init; }
    public int? MeleeAttackDamage { get; init; }
    public int? BowAttackDamage { get; init; }
    public int Movement { get; init; }
    public int MaxConsumableInventory { get; init; }
    public int MaxEquippableInventory { get; init; }
    public int MaxHp { get; init; }
    public int MaxMp { get; init; }
    public bool IsInitiallyActive { get; init; }
    public string? AlternateFormCharacterKey { get; init; }
    public List<AssignedSpellDefinitionSnapshot> Spells { get; init; } = [];
}

public sealed class SceneCharacterDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceSceneCharacterId { get; init; }
    public required string CharacterKey { get; init; }
    public int? MeleeAttackDamage { get; init; }
    public int? BowAttackDamage { get; init; }
    public int Movement { get; init; }
    public int MaxConsumableInventory { get; init; }
    public int MaxEquippableInventory { get; init; }
    public int MaxHp { get; init; }
    public int MaxMp { get; init; }
    public bool IsInitiallyActive { get; init; }
    public string? AlternateFormCharacterKey { get; init; }
    public List<AssignedSpellDefinitionSnapshot> Spells { get; init; } = [];
}

public sealed class AssignedSpellDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceAssignmentId { get; init; }
    public required string SpellKey { get; init; }
}

public sealed class SpellTypeDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceSpellTypeId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string PhotoUrl { get; init; }
    public required string FileName { get; init; }
}

public sealed class SpellDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceSpellId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string? PhotoUrl { get; init; }
    public string? FileName { get; init; }
    public int Range { get; init; }
    public bool IsRadius { get; init; }
    public int MpCost { get; init; }
    public int? DamageEffect { get; init; }
    public int? HealthEffect { get; init; }
    public int? MagicEffect { get; init; }
    public required string SpellTypeKey { get; init; }
}

public sealed class ConsumableDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceConsumableId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string PhotoUrl { get; init; }
    public required string FileName { get; init; }
    public int HpEffect { get; init; }
    public int MpEffect { get; init; }
}

public sealed class EquipmentDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceEquipmentId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string PhotoUrl { get; init; }
    public required string FileName { get; init; }
    public int MeleeAttackDamageModifier { get; init; }
    public int BowAttackDamageModifier { get; init; }
    public int MovementModifier { get; init; }
    public int MaxHpModifier { get; init; }
    public int MaxMpModifier { get; init; }
    public int MaxConsumableInventoryModifier { get; init; }
    public int MaxEquippableInventoryModifier { get; init; }
    public int MeleeDamageReduction { get; init; }
    public int BowDamageReduction { get; init; }
    public int SpellDamageReduction { get; init; }
    public string? AffectedSpellTypeKey { get; init; }
    public int? SpellDamageModifier { get; init; }
    public List<string> AddedSpellKeys { get; init; } = [];
}

public sealed class SceneDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceSceneId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? PhotoUrl { get; init; }
    public string? FileName { get; init; }
    public string? GridUrl { get; init; }
    public int SortOrder { get; init; }
    public SceneGridDefinitionSnapshot? Grid { get; init; }
    public List<IntroPageDefinitionSnapshot> IntroPages { get; init; } = [];
    public List<SceneCharacterDefinitionSnapshot> Characters { get; init; } = [];
    public List<DialogDefinitionSnapshot> Dialogs { get; init; } = [];
    public List<EventDefinitionSnapshot> Events { get; init; } = [];
    public List<ChestDefinitionSnapshot> Chests { get; init; } = [];
}

public sealed class SceneGridDefinitionSnapshot
{
    public int Rows { get; init; }
    public int Columns { get; init; }
    public required string GridColor { get; init; }
    public string? BackgroundImageUrl { get; init; }
    public string? BackgroundFileName { get; init; }
}

public sealed class DialogDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceDialogId { get; init; }
    public required string Title { get; init; }
    public List<DialogPageDefinitionSnapshot> Pages { get; init; } = [];
}

public sealed class DialogPageDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourcePageId { get; init; }
    public int OrderNumber { get; init; }
    public string? PhotoUrl { get; init; }
    public string? FileName { get; init; }
    public List<DialogSectionDefinitionSnapshot> Sections { get; init; } = [];
}

public sealed class DialogSectionDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceSectionId { get; init; }
    public int OrderNumber { get; init; }
    public required string ReadingText { get; init; }
    public bool IsNarrator { get; init; }
    public string? CharacterKey { get; init; }
}

public sealed class EventDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceEventId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public int SortOrder { get; init; }
    public List<EventActionDefinitionSnapshot> Actions { get; init; } = [];
}

public sealed class EventActionDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceActionId { get; init; }
    public required string Name { get; init; }
    public int SortOrder { get; init; }
    public ActionTargetType TargetType { get; init; }
    public EventActionType ActionType { get; init; }
    public CharacterStatAdjustmentDefinitionSnapshot? CharacterStatAdjustment { get; init; }
}

public sealed class CharacterStatAdjustmentDefinitionSnapshot
{
    public CharacterStatType StatType { get; init; }
    public AdjustmentOperation Operation { get; init; }
    public int Value { get; init; }
    public string? CharacterKey { get; init; }
}

public sealed class ChestDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceChestId { get; init; }
    public required string Name { get; init; }
    public int DieSides { get; init; }
    public List<LootEntryDefinitionSnapshot> LootEntries { get; init; } = [];
}

public sealed class LootEntryDefinitionSnapshot
{
    public required string Key { get; init; }
    public int SourceLootEntryId { get; init; }
    public int RollMinimum { get; init; }
    public int RollMaximum { get; init; }
    public int Quantity { get; init; }
    public string? EquipmentKey { get; init; }
    public string? ConsumableKey { get; init; }
}
