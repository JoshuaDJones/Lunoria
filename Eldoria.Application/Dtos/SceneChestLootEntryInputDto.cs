namespace Eldoria.Application.Dtos;

public class SceneChestLootEntryInputDto
{
    public int RollMinimum { get; set; }
    public int RollMaximum { get; set; }
    public int Quantity { get; set; }
    public int? EquippableItemId { get; set; }
    public int? ConsumableItemId { get; set; }
}
