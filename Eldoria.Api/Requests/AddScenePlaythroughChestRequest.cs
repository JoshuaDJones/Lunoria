using System.ComponentModel.DataAnnotations;
using Eldoria.Application.Dtos;

namespace Eldoria.Api.Requests;

public sealed class AddScenePlaythroughChestRequest
{
    [Required, MaxLength(250)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 6)]
    public int DieSides { get; set; }

    [Required, MinLength(1), MaxLength(6)]
    public List<AddScenePlaythroughChestLootEntryRequest> LootEntries { get; set; } = [];

    public CreateScenePlaythroughChestDto ToDto() => new()
    {
        Name = Name,
        DieSides = DieSides,
        LootEntries = [.. LootEntries.Select(entry => entry.ToDto())]
    };
}

public sealed class AddScenePlaythroughChestLootEntryRequest
{
    [Range(1, 6)]
    public int RollMinimum { get; set; }

    [Range(1, 6)]
    public int RollMaximum { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(1, int.MaxValue)]
    public int? PlaythroughEquippableItemId { get; set; }

    [Range(1, int.MaxValue)]
    public int? PlaythroughConsumableItemId { get; set; }

    public CreateScenePlaythroughChestLootEntryDto ToDto() => new()
    {
        RollMinimum = RollMinimum,
        RollMaximum = RollMaximum,
        Quantity = Quantity,
        PlaythroughEquippableItemId = PlaythroughEquippableItemId,
        PlaythroughConsumableItemId = PlaythroughConsumableItemId
    };
}
