namespace Eldoria.Core.Entities
{
    public class JourneyPlaythroughCharacterConsumableItem
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public string SnapshotConsumableKey { get; set; } = string.Empty;

        public int? ConsumableItemId { get; set; }
        public ConsumableItem? ConsumableItem { get; set; }

        public int JourneyPlaythroughCharacterId { get; set; }
        public JourneyPlaythroughCharacter JourneyPlaythroughCharacter { get; set; } = null!;
    }
}
