namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughCharacterEquippableItemDto
    {
        public int Id { get; set; }
        public bool IsEquipped { get; set; }
        public int ScenePlaythroughCharacterId { get; set; }
        public int? SourceEquippableItemId { get; set; }
        public string SnapshotEquipmentKey { get; set; } = string.Empty;
    }
}
