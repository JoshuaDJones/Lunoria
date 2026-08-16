using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTParticipant
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrderWithinType { get; set; }
        public ParticipantType ParticipantType { get; set; }

        public int ScenePlaythroughId { get; set; }
        public ScenePT ScenePlaythrough { get; set; } = null!;

        public int? JourneyPlaythroughCharacterId { get; set; }
        public JourneyPTCharacter? JourneyPlaythroughCharacter { get; set; }

        public int? ScenePlaythroughCharacterId { get; set; }
        public ScenePTCharacter? ScenePlaythroughCharacter { get; set; }
    }
}
