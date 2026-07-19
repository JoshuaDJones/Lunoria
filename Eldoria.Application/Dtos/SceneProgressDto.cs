using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class SceneProgressDto
    {
        public int Id { get; set; }
        public int SceneId { get; set; }
        public int JourneyPlaythroughId { get; set; }
        public ScenePlaythroughStatus Status { get; set; }
        public List<SceneParticipantDto> Participants { get; set; } = [];
        public List<SceneParticipantTurnDto> Turns { get; set; } = [];
    }
}
