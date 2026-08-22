using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class Playthrough
    {
        public int Id { get; set; }
        public int SourceJourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<PlaythroughCharacter> Characters { get; set; } = [];
        public ICollection<PlaythroughEventLog> EventLogs { get; set; } = [];
        public ICollection<PlaythroughIntroPage> IntroPages { get; set; } = [];
        public ICollection<PlaythroughSpellType> SpellTypes { get; set; } = [];
        public ICollection<PlaythroughSpell> Spells { get; set; } = [];
        public ICollection<PlaythroughConsumableItem> ConsumableItems { get; set; } = [];
        public ICollection<PlaythroughEquippableItem> EquippableItems { get; set; } = [];
        public ICollection<JourneyPTCharacter> JourneyCharacters { get; set; } = [];
        public ICollection<ScenePT> Scenes { get; set; } = [];
    }
}
