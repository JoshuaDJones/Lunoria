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

    public static PlaythroughStartDto ToStartDto(this Playthrough playthrough)
    {
        return new PlaythroughStartDto
        {
            Playthrough = playthrough.ToSummaryDto(),
            Scenes = playthrough.Scenes
                .OrderBy(scene => scene.SortOrder)
                .ThenBy(scene => scene.Id)
                .Select(scene => new PlaythroughSceneSummaryDto
                {
                    Id = scene.Id,
                    Name = scene.Name,
                    Description = scene.Description,
                    PhotoUrl = scene.PhotoUrl,
                    GridUrl = scene.GridUrl,
                    SortOrder = scene.SortOrder,
                    Status = scene.Status,
                    RoundNumber = scene.RoundNumber,
                    StartedAt = scene.StartedAt,
                    EndedAt = scene.EndedAt
                })
                .ToList(),
            IntroPages = playthrough.IntroPages
                .OrderBy(introPage => introPage.SortOrder)
                .ThenBy(introPage => introPage.Id)
                .Select(introPage => new PlaythroughIntroPageDto
                {
                    Id = introPage.Id,
                    SortOrder = introPage.SortOrder,
                    Type = introPage.Type,
                    Config = introPage.Config,
                    PreviewPhotoUrl = introPage.PreviewPhotoUrl
                })
                .ToList(),
            EventLogs = playthrough.EventLogs
                .OrderBy(eventLog => eventLog.EventTime)
                .ThenBy(eventLog => eventLog.Id)
                .Select(eventLog => new PlaythroughEventLogDto
                {
                    Id = eventLog.Id,
                    Message = eventLog.Message,
                    EventTime = eventLog.EventTime
                })
                .ToList()
        };
    }
}
