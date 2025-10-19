namespace Eldoria.Application.Dtos
{
    public class SceneDto
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string GridUrl { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }

        public List<SceneDialogDto>? SceneDialogs { get; set; } = [];
        public List<SceneCharacterDto>? SceneCharacters { get; set; } = [];
    }
}
