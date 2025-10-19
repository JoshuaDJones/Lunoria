namespace Eldoria.Core.Entities
{
    public class DialogPageSection
    {
        public int Id { get; set; }
        public int OrderNum { get; set; }
        public string ReadingText { get; set; } = string.Empty;
        public bool IsNarrator { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        public int DialogPageId { get; set; }
        public DialogPage DialogPage { get; set; } = null!;
    }
}
