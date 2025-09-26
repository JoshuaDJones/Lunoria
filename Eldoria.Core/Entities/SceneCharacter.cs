namespace Eldoria.Core.Entities
{
    public class SceneCharacter
    {
        public int Id { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public bool IsDown { get; set; }
        public bool IsAlternateForm { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public ICollection<SceneCharacterItem> SceneCharacterItems { get; set; } = [];
    }
}
