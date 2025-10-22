namespace Eldoria.Api.Requests
{
    public class UpdateDialogPageSectionRequest
    {
        public int? CharacterId { get; set; }
        public int? OrderNum { get; set; }
        public string? ReadingText { get; set; }
        public bool? IsNarrator { get; set; }
    }
}
