using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class ScenePlaythrough
    {
        public int Id { get; set; }
        public ScenePlaythroughStatus Status { get; set; } = ScenePlaythroughStatus.NotStarted;
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? CurrentParticipantId { get; set; }
        public ScenePlaythroughParticipant? CurrentParticipant { get; set; }
        public int RoundNumber { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public int JourneyPlaythroughId { get; set; }
        public JourneyPlaythrough JourneyPlaythrough { get; set; } = null!;

        public ICollection<ScenePlaythroughCharacter> SceneCharacters { get; set; } = [];
        public ICollection<ScenePlaythroughParticipant> Participants { get; set; } = [];
        public ICollection<ScenePlaythroughEvent> PlaythroughEvents { get; set; } = [];
        public ICollection<ScenePlaythroughChest> PlaythroughChests { get; set; } = [];
    }
}
