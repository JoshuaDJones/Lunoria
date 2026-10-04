namespace Eldoria.Application.Dtos;

public sealed record CharacterRevisionStatusDto(int CurrentRevision, int? SyncedRevision, int? AcknowledgedRevision)
{
    public bool RequiresReview => AcknowledgedRevision is null;
    public bool UpdateAvailable => AcknowledgedRevision.HasValue && AcknowledgedRevision != CurrentRevision;
}

