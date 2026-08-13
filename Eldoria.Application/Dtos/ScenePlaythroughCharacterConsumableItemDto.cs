namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughCharacterConsumableItemDto
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public int ScenePlaythroughCharacterId { get; set; }
        public int? SourceConsumableItemId { get; set; }
        public string SnapshotConsumableKey { get; set; } = string.Empty;
    }
}
