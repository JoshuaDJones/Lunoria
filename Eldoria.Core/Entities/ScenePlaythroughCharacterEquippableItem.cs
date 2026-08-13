namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughCharacterEquippableItem
    {
        public int Id { get; set; }
        public string SnapshotEquipmentKey { get; set; } = string.Empty;
        public bool IsEquipped { get; set; }

        public int? EquippableItemId { get; set; }
        public EquippableItem? EquippableItem { get; set; }

        public int ScenePlaythroughCharacterId { get; set; }
        public ScenePlaythroughCharacter ScenePlaythroughCharacter { get; set; } = null!;
    }
}
