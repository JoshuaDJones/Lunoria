using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Application.Common;

public static class JourneyPlaythroughEventLogMappings
{
    public static JourneyPlaythroughEventLogDto ToDto(this PlaythroughEventLog eventLog) => new()
    {
        Id = eventLog.Id,
        Message = eventLog.Message,
        EventTime = eventLog.EventTime,
        JourneyPlaythrough = eventLog.JourneyPlaythrough.ToDto()
    };
}
