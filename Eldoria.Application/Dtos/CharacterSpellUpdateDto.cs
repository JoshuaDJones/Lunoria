namespace Eldoria.Application.Dtos;

public sealed record CharacterSpellUpdateDto(int SpellId, string Name, int CurrentRevision, int? AcknowledgedRevision, bool IsArchived)
{
    public bool RequiresReview => AcknowledgedRevision is null;
    public bool UpdateAvailable => AcknowledgedRevision.HasValue && AcknowledgedRevision != CurrentRevision;
}

