namespace Eldoria.Application.Dtos;

public sealed record CharacterSyncPreviewDto(
    int AssignmentId,
    int CharacterId,
    CharacterSyncStatusDto Status,
    CharacterSyncStatsDto AssignedStats,
    CharacterSyncStatsDto BaseStats,
    int? AssignedAlternateFormId,
    int? BaseAlternateFormId,
    List<SpellDto> AssignedSpells,
    List<SpellDto> BaseSpells,
    string ReviewToken,
    string SharedSpellNotice);

