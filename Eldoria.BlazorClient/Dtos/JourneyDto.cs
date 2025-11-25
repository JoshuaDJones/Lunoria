namespace Eldoria.BlazorClient.Dtos
{
    public class JourneyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public List<SceneDto>? Scenes { get; set; } = [];
        public List<JourneyCharacterDto>? JourneyCharacters { get; set; } = [];
    }
}
