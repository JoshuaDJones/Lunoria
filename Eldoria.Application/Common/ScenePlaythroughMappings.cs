using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class ScenePlaythroughMappings
    {
        public static ScenePlaythroughDto ToDto(this ScenePlaythrough playthrough)
        {
            return new ScenePlaythroughDto
            {
                Id = playthrough.Id,
                Status = playthrough.Status,
                StartedAt = playthrough.StartedAt,
                EndedAt = playthrough.EndedAt,
                CurrentParticipantId = playthrough.CurrentParticipantId,
                CurrentParticipant = playthrough.CurrentParticipant?.ToDto(),
                RoundNumber = playthrough.RoundNumber,
                Scene = playthrough.Scene.ToDto(),
                JourneyPlaythrough = playthrough.JourneyPlaythrough.ToDto(),
                SceneCharacters = playthrough.SceneCharacters.Select(sc => sc.ToDto()).ToList(),
                Participants = playthrough.Participants.Select(p => p.ToDto()).ToList(),
                PlaythroughEvents = playthrough.PlaythroughEvents.Select(e => e.ToDto()).ToList(),
                PlaythroughChests = playthrough.PlaythroughChests.Select(c => c.ToDto()).ToList()
            };
        }
    }
}