using Eldoria.Core.Entities.Playthrough.Journey;

using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class PlaythroughSpell
    {
        public int Id { get; set; }
        public int SourceSpellId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? FileName { get; set; }
        public int Range { get; set; }
        public bool IsRadius { get; set; }
        public int MpCost { get; set; }
        public int? DamageEffect { get; set; }
        public int? HealthEffect { get; set; }
        public int? MagicEffect { get; set; }

        public int PlaythroughSpellTypeId { get; set; }
        public PlaythroughSpellType PlaythroughSpellType { get; set; } = null!;

        public int PlaythroughId { get; set; }
        public Playthrough Playthrough { get; set; } = null!;

        public ICollection<PlaythroughCharacterSpell> BaseCharacterSpells { get; set; } = [];
        public ICollection<JourneyPTCharacterSpell> JourneyCharacterSpells { get; set; } = [];
        public ICollection<ScenePTCharacterSpell> SceneCharacterSpells { get; set; } = [];
        public ICollection<PlaythroughEquippableItem> EquippableItems { get; set; } = [];
    }
}
