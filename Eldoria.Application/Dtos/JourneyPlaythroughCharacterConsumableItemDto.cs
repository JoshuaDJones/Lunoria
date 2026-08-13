namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughCharacterConsumableItemDto
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public int JourneyPlaythroughCharacterId { get; set; }
        public int? SourceConsumableItemId { get; set; }
        public string SnapshotConsumableKey { get; set; } = string.Empty;
    }
}
