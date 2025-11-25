namespace Eldoria.BlazorClient.Dtos
{
    public class DialogPageSectionDto
    {
        public int Id { get; set; }
        public int OrderNum { get; set; }
        public string ReadingText { get; set; } = string.Empty;
        public bool IsNarrator { get; set; }
        public CharacterDto? Character { get; set; }
    }
}
