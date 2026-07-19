namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughCharacterEquippableItem
    {
        public int Id { get; set; }

        public int EquippableItemId { get; set; }
        public EquippableItem EquippableItem { get; set; } = null!;

        public int ScenePlaythroughCharacterId { get; set; }
        public ScenePlaythroughCharacter ScenePlaythroughCharacter { get; set; } = null!;
    }
}
