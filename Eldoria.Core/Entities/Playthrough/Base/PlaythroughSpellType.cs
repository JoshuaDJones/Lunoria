namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class PlaythroughSpellType
    {
        public int Id { get; set; }
        public int SourceSpellTypeId { get; set; }
        public required string TypeName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public int PlaythroughId { get; set; }
        public Playthrough Playthrough { get; set; } = null!;

        public ICollection<PlaythroughSpell> Spells { get; set; } = [];
        public ICollection<PlaythroughEquippableItem> AffectedEquippableItems { get; set; } = [];
    }
}
