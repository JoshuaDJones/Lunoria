namespace Eldoria.Core.Entities
{
    public class Scene
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string GridUrl { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int JourneyId { get; set; }
        public Journey Journey { get; set; } = null!;

        public ICollection<SceneDialog> SceneDialogs { get; set; } = [];
        public ICollection<SceneCharacter> SceneCharacters { get; set; } = [];
    }
}
