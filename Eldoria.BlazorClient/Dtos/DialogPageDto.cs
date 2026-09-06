namespace Eldoria.BlazorClient.Dtos
{
    public enum DialogPageType
    {
        Image = 1,
        Video = 2
    }

    public class DialogPageDto
    {
        public int Id { get; set; }
        public int OrderNum { get; set; }
        public DialogPageType PageType { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaContentType { get; set; } = string.Empty;
        public List<DialogPageSectionDto>? DialogPageSections { get; set; } = [];
    }
}
