namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughCharacterEquippableItemDto
    {
        public int Id { get; set; }
        public bool IsEquipped { get; set; }
        public int JourneyPlaythroughCharacterId { get; set; }
        public int? SourceEquippableItemId { get; set; }
        public string SnapshotEquipmentKey { get; set; } = string.Empty;
    }
}
