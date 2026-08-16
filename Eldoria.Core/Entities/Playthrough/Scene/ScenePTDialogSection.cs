using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTDialogSection
    {
        public int Id { get; set; }
        public int SourceDialogSectionId { get; set; }
        public int OrderNum { get; set; }
        public string ReadingText { get; set; } = string.Empty;
        public bool IsNarrator { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int? CharacterId { get; set; }
        public PlaythroughCharacter? Character { get; set; }

        public int DialogPageId { get; set; }
        public ScenePTDialogPage DialogPage { get; set; } = null!;
    }
}
