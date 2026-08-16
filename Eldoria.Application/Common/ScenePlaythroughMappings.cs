using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common
{
    public static class ScenePlaythroughMappings
    {
        public static ScenePlaythroughDto ToDto(this ScenePT playthrough)
        {
            return new ScenePlaythroughDto
            {
                Id = playthrough.Id,
                Status = playthrough.Status,
                StartedAt = playthrough.StartedAt,
                EndedAt = playthrough.EndedAt,
                CurrentParticipantId = playthrough.CurrentParticipantId,
                RoundNumber = playthrough.RoundNumber,
                SourceSceneId = playthrough.SourceSceneId,
                SnapshotSceneKey = playthrough.SnapshotSceneKey,
                SnapshotSortOrder = playthrough.SnapshotSortOrder,
                JourneyPlaythroughId = playthrough.JourneyPlaythroughId,
                SceneCharacters = playthrough.SceneCharacters.Select(sc => sc.ToDto()).ToList(),
                Participants = playthrough.Participants.Select(p => p.ToDto()).ToList(),
                PlaythroughEvents = playthrough.PlaythroughEvents.Select(e => e.ToDto()).ToList(),
                PlaythroughChests = playthrough.PlaythroughChests.Select(c => c.ToDto()).ToList()
            };
        }
    }
}
