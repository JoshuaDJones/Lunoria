using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyPlaythroughMappings
    {
        public static JourneyPlaythroughDto ToDto(this JourneyPlaythrough playthrough)
        {
            return new JourneyPlaythroughDto
            {
                Id = playthrough.Id,
                JourneyId = playthrough.JourneyId,
                StartedAt = playthrough.StartedAt,
                CompletedAt = playthrough.CompletedAt,
                IsActive = playthrough.IsActive,
            };
        }
    }
}
