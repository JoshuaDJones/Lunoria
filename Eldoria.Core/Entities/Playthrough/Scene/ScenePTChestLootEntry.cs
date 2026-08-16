using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTChestLootEntry
    {
        public int Id { get; set; }
        public int SourceSceneChestLootEntryId { get; set; }
        public int RollMinimum { get; set; }
        public int RollMaximum { get; set; }
        public int Quantity { get; set; }

        public int? PlaythroughEquippableItemId { get; set; }
        public PlaythroughEquippableItem? PlaythroughEquippableItem { get; set; }

        public int? PlaythroughConsumableItemId { get; set; }
        public PlaythroughConsumableItem? PlaythroughConsumableItem { get; set; }

        public int ScenePTChestId { get; set; }
        public ScenePTChest ScenePTChest { get; set; } = null!;
    }
}
