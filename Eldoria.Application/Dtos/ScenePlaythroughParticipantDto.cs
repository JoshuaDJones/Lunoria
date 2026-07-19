using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughParticipantDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrderWithinType { get; set; }
        public ParticipantType ParticipantType { get; set; }
        public ScenePlaythroughDto ScenePlaythrough { get; set; } = null!;
        public JourneyPlaythroughCharacterDto? JourneyPlaythroughCharacter { get; set; }
        public ScenePlaythroughCharacterDto? ScenePlaythroughCharacter { get; set; }
    }
}
