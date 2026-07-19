namespace Eldoria.Core.Entities
{
    public class SceneChestLootEntry
    {
        public int Id { get; set; }
        public int RollMinimum { get; set; }
        public int RollMaximum { get; set; }
        public int Quantity { get; set; }

        public int? EquippableItemId { get; set; }
        public EquippableItem? EquippableItem { get; set; }

        public int? ConsumableItemId { get; set; }
        public ConsumableItem? ConsumableItem { get; set; }

        public int SceneChestId { get; set; }
        public SceneChest SceneChest { get; set; } = null!;
    }
}
