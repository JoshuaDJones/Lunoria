using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class ScenePlaythroughParticipantMappings
    {
        public static ScenePlaythroughParticipantDto ToDto(
    this ScenePlaythroughParticipant participant)
        {
            return new ScenePlaythroughParticipantDto
            {
                Id = participant.Id,
                IsActive = participant.IsActive,
                SortOrderWithinType = participant.SortOrderWithinType,
                ParticipantType = participant.ParticipantType,
                ScenePlaythrough = participant.ScenePlaythrough.ToDto(),
                JourneyPlaythroughCharacter = participant.JourneyPlaythroughCharacter?.ToDto(),
                ScenePlaythroughCharacter = participant.ScenePlaythroughCharacter?.ToDto()
            };
        }
    }
}