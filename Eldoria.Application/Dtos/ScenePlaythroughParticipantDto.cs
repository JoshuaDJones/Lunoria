using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughParticipantDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrderWithinType { get; set; }
        public ParticipantType ParticipantType { get; set; }
        public int ScenePlaythroughId { get; set; }
        public int? JourneyPlaythroughCharacterId { get; set; }
        public int? ScenePlaythroughCharacterId { get; set; }
    }
}
