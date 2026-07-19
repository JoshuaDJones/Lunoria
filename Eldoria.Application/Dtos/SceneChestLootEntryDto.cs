using Eldoria.Core.Entities;

namespace Eldoria.Application.Dtos
{
    public class SceneChestLootEntryDto
    {
        public int Id { get; set; }
        public int RollMinimum { get; set; }
        public int RollMaximum { get; set; }
        public int Quantity { get; set; }
        public EquippableItemDto? EquippableItem { get; set; }
        public ConsumableItemDto? ConsumableItem { get; set; }
        public SceneChestDto SceneChest { get; set; } = null!;
    }
}
