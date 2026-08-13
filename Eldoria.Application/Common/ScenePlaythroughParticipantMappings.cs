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
                ScenePlaythroughId = participant.ScenePlaythroughId,
                JourneyPlaythroughCharacterId = participant.JourneyPlaythroughCharacterId,
                ScenePlaythroughCharacterId = participant.ScenePlaythroughCharacterId
            };
        }
    }
}
