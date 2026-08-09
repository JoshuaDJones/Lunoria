using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class SceneChestRequest
    {
        [Required, MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [Required, Range(1, int.MaxValue)]
        public int? DieSides { get; set; }
    }

    public class SceneChestLootEntryRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int? RollMinimum { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int? RollMaximum { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int? Quantity { get; set; }

        [Range(1, int.MaxValue)]
        public int? EquippableItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int? ConsumableItemId { get; set; }
    }
}
