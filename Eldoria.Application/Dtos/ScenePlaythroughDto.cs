using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughDto
    {
        public int Id { get; set; }
        public ScenePlaythroughStatus Status { get; set; } = ScenePlaythroughStatus.NotStarted;
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? CurrentParticipantId { get; set; }
        public ScenePlaythroughParticipantDto? CurrentParticipant { get; set; }
        public int RoundNumber { get; set; }
        public SceneDto Scene { get; set; } = null!;
        public JourneyPlaythroughDto JourneyPlaythrough { get; set; } = null!;
        public List<ScenePlaythroughCharacterDto> SceneCharacters { get; set; } = [];
        public List<ScenePlaythroughParticipantDto> Participants { get; set; } = [];
        public List<ScenePlaythroughEventDto> PlaythroughEvents { get; set; } = [];
        public List<ScenePlaythroughChestDto> PlaythroughChests { get; set; } = [];
    }
}
