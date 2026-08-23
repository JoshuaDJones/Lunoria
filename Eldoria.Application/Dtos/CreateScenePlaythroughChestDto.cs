namespace Eldoria.Application.Dtos;

public sealed class CreateScenePlaythroughChestDto
{
    public string Name { get; set; } = string.Empty;
    public int DieSides { get; set; }
    public List<CreateScenePlaythroughChestLootEntryDto> LootEntries { get; set; } = [];
}

public sealed class CreateScenePlaythroughChestLootEntryDto
{
    public int RollMinimum { get; set; }
    public int RollMaximum { get; set; }
    public int Quantity { get; set; }
    public int? PlaythroughEquippableItemId { get; set; }
    public int? PlaythroughConsumableItemId { get; set; }
}
