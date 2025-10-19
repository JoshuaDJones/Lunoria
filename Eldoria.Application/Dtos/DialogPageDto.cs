namespace Eldoria.Application.Dtos
{
    public class DialogPageDto
    {
        public int Id { get; set; }
        public int OrderNum { get; set; }
        public string? PhotoUrl { get; set; }
        public List<DialogPageSectionDto>? DialogPageSections { get; set; } = [];
    }
}
