namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughChestLootEntryDto
{
    public int Id { get; set; }
    public int RollMinimum { get; set; }
    public int RollMaximum { get; set; }
    public int Quantity { get; set; }
    public ScenePlaythroughLootItemDto? EquippableItem { get; set; }
    public ScenePlaythroughLootItemDto? ConsumableItem { get; set; }
}
