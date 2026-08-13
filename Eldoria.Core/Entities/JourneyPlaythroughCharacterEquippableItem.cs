namespace Eldoria.Core.Entities
{
    public class JourneyPlaythroughCharacterEquippableItem
    {
        public int Id { get; set; }
        public string SnapshotEquipmentKey { get; set; } = string.Empty;
        public bool IsEquipped { get; set; }

        public int? EquippableItemId { get; set; }
        public EquippableItem? EquippableItem { get; set; }

        public int JourneyPlaythroughCharacterId { get; set; }
        public JourneyPlaythroughCharacter JourneyPlaythroughCharacter { get; set; } = null!;
    }
}
