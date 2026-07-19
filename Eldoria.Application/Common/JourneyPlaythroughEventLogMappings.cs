using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class JourneyPlaythroughEventLogMappings
{
    public static JourneyPlaythroughEventLogDto ToDto(this JourneyPlaythroughEventLog eventLog) => new()
    {
        Id = eventLog.Id,
        Message = eventLog.Message,
        EventTime = eventLog.EventTime,
        JourneyPlaythrough = eventLog.JourneyPlaythrough.ToDto()
    };
}
