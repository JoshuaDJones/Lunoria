namespace Eldoria.Core.Entities
{
    public class Scene
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public string? GridUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int JourneyId { get; set; }
        public Journey Journey { get; set; } = null!;

        public ICollection<SceneChest> SceneChests { get; set; } = [];
        public ICollection<SceneIntroPage> SceneIntroPages { get; set; } = [];
        public ICollection<SceneEvent> SceneEvents { get; set; } = [];
        public ICollection<ScenePlaythrough> ScenePlaythroughs { get; set; } = [];
        public ICollection<SceneDialog> SceneDialogs { get; set; } = [];
        public ICollection<SceneCharacter> SceneCharacters { get; set; } = [];
    }
}
