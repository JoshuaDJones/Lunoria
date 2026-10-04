namespace Eldoria.Application.Dtos;

public sealed record CharacterSyncStatusDto(
    bool BaseAvailable,
    CharacterRevisionStatusDto Stats,
    CharacterRevisionStatusDto SpellAssignments,
    CharacterRevisionStatusDto AlternateForm,
    List<CharacterSpellUpdateDto> SharedSpells)
{
    public bool RequiresReview => Stats.RequiresReview || SpellAssignments.RequiresReview || AlternateForm.RequiresReview ||
        SharedSpells.Any(spell => spell.RequiresReview);
    public bool HasUpdates => Stats.UpdateAvailable || SpellAssignments.UpdateAvailable || AlternateForm.UpdateAvailable ||
        SharedSpells.Any(spell => spell.UpdateAvailable);
}

