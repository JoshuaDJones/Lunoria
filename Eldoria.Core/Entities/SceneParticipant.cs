namespace Eldoria.Core.Entities
{
    public class SceneParticipant
    {
        public int Id { get; set; }

        public int SceneProgressId { get; set; }
        public SceneProgress SceneProgress { get; set; } = null!;

        public int? JourneyCharacterId { get; set; }
        public JourneyCharacter? JourneyCharacter { get; set; }

        public int? SceneCharacterId { get; set; }
        public SceneCharacter? SceneCharacter { get; set; }

        public ICollection<SceneParticipantTurn> Turns { get; set; } = [];
    }
}
