namespace Eldoria.Core.Entities
{
    public class SceneCharacterItem
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }

        public int SceneCharacterId { get; set; }
        public SceneCharacter SceneCharacter { get; set; } = null!;

        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;
    }
}
