namespace Eldoria.Application.Dtos;

public sealed record CharacterSyncSelectionDto(
    string ExpectedReviewToken,
    bool Stats,
    bool SpellAssignments,
    bool AlternateForm,
    bool SharedSpellUpdates);

