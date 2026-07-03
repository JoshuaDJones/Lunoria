using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SceneProgressMappings
    {
        public static SceneProgressDto ToDto(this SceneProgress progress)
        {
            return new SceneProgressDto
            {
                Id = progress.Id,
                SceneId = progress.SceneId,
                JourneyPlaythroughId = progress.JourneyPlaythroughId,
                Status = progress.SceneProgressStatus,
                Participants = progress.Participants
                    .OrderBy(participant => participant.Id)
                    .Select(participant => participant.ToDto())
                    .ToList(),
                Turns = progress.ParticipantTurns
                    .OrderBy(turn => turn.TurnPosition)
                    .Select(turn => turn.ToDto())
                    .ToList(),
            };
        }

        public static SceneParticipantDto ToDto(this SceneParticipant participant)
        {
            return new SceneParticipantDto
            {
                Id = participant.Id,
                SceneProgressId = participant.SceneProgressId,
                JourneyCharacterId = participant.JourneyCharacterId,
                SceneCharacterId = participant.SceneCharacterId,
                Turns = participant.Turns
                    .OrderBy(turn => turn.TurnPosition)
                    .Select(turn => turn.ToDto())
                    .ToList(),
            };
        }

        public static SceneParticipantTurnDto ToDto(this SceneParticipantTurn turn)
        {
            return new SceneParticipantTurnDto
            {
                Id = turn.Id,
                SceneProgressId = turn.SceneProgressId,
                SceneParticipantId = turn.SceneParticipantId,
                TurnPosition = turn.TurnPosition,
            };
        }
    }
}
