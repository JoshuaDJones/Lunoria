namespace Eldoria.Core.Entities
{
    public class Journey
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Scene> Scenes { get; set; } = [];
        public ICollection<JourneyCharacter> JourneyCharacters { get; set; } = [];
    }
}
