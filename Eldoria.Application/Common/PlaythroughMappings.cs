using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Application.Common;

public static class PlaythroughMappings
{
    public static PlaythroughSummaryDto ToSummaryDto(this Playthrough playthrough)
    {
        return new PlaythroughSummaryDto
        {
            Id = playthrough.Id,
            SourceJourneyId = playthrough.SourceJourneyId,
            Name = playthrough.Name,
            Description = playthrough.Description,
            PhotoUrl = playthrough.PhotoUrl,
            StartedAt = playthrough.StartedAt,
            CompletedAt = playthrough.CompletedAt,
            IsCompleted = playthrough.CompletedAt is not null
        };
    }
}
