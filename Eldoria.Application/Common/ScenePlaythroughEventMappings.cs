using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughEventMappings
{
    public static ScenePlaythroughEventDto ToDto(this ScenePlaythroughEvent playthroughEvent) => new()
    {
        Id = playthroughEvent.Id,
        ExecutionStatus = playthroughEvent.ExecutionStatus,
        ErrorMessage = playthroughEvent.ErrorMessage,
        StartedAt = playthroughEvent.StartedAt,
        CompletedAt = playthroughEvent.CompletedAt,
        ScenePlaythrough = playthroughEvent.ScenePlaythrough.ToDto(),
        SceneEvent = playthroughEvent.SceneEvent.ToDto()
    };
}
