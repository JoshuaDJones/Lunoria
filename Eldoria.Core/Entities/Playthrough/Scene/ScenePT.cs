using Eldoria.Core.Enums;
using PlaythroughEntity = Eldoria.Core.Entities.Playthrough.Base.Playthrough;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePT
    {
        public int Id { get; set; }
        public int SourceSceneId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public string? GridUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }

        public ScenePlaythroughStatus Status { get; set; } = ScenePlaythroughStatus.NotStarted;        
        public int RoundNumber { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public int? CurrentParticipantId { get; set; } 
        public ScenePTParticipant? CurrentParticipant { get; set; }

        public int PlaythroughId { get; set; }
        public PlaythroughEntity Playthrough { get; set; } = null!;

        public ScenePTGrid? ScenePTGrid { get; set; } = null!;

        public ICollection<ScenePTIntroPage> IntroPages { get; set; } = [];
        public ICollection<ScenePTCharacter> SceneCharacters { get; set; } = [];
        public ICollection<ScenePTParticipant> SceneParticipants { get; set; } = [];
        public ICollection<ScenePTEvent> SceneEvents { get; set; } = [];
        public ICollection<ScenePTChest> SceneChests { get; set; } = [];
        public ICollection<ScenePTDialog> SceneDialogs { get; set; } = [];
    }
}
