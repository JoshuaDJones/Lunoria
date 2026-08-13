namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughCharacterConsumableItem
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public string SnapshotConsumableKey { get; set; } = string.Empty;

        public int? ConsumableItemId { get; set; }
        public ConsumableItem? ConsumableItem { get; set; }

        public int ScenePlaythroughCharacterId { get; set; }
        public ScenePlaythroughCharacter ScenePlaythroughCharacter { get; set; } = null!;
    }
}
