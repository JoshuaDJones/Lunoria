using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughParticipant
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrderWithinType { get; set; }
        public ParticipantType ParticipantType { get; set; }

        public int ScenePlaythroughId { get; set; }
        public ScenePlaythrough ScenePlaythrough { get; set; } = null!;

        public int? JourneyPlaythroughCharacterId { get; set; }
        public JourneyPlaythroughCharacter? JourneyPlaythroughCharacter { get; set; }

        public int? ScenePlaythroughCharacterId { get; set; }
        public ScenePlaythroughCharacter? ScenePlaythroughCharacter { get; set; }
    }
}
