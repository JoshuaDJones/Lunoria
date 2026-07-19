using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SceneChestLootEntryMappings
{
    public static SceneChestLootEntryDto ToDto(this SceneChestLootEntry entry) => new()
    {
        Id = entry.Id,
        RollMinimum = entry.RollMinimum,
        RollMaximum = entry.RollMaximum,
        Quantity = entry.Quantity,
        EquippableItem = entry.EquippableItem?.ToDto(),
        ConsumableItem = entry.ConsumableItem?.ToDto(),
        SceneChest = entry.SceneChest.ToDto()
    };
}
