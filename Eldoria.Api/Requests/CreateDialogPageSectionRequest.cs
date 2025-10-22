namespace Eldoria.Api.Requests
{
    public class CreateDialogPageSectionRequest
    {
        public int CharacterId { get; set; }        
        public int OrderNum { get; set; }
        public string ReadingText { get; set; } = string.Empty;
        public bool IsNarrator { get; set; }
    }
}
