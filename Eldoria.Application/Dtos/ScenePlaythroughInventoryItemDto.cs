namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughInventoryItemDto
{
    public int InventoryItemId { get; set; }
    public bool IsEquippable { get; set; }
    public bool IsEquipped { get; set; }
    public ScenePlaythroughLootItemDto Item { get; set; } = new();
}
