namespace Eldoria.Core.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int HpEffect { get; set; }
        public int MpEffect { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public ICollection<SceneCharacterItem> SceneCharacterItems { get; set; } = [];
        public ICollection<JourneyCharacterItem> JourneyCharacterItems { get; set; } = [];
    }
}
