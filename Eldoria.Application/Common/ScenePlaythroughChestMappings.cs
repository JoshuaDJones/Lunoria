using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughChestMappings
{
    public static ScenePlaythroughChestDto ToDto(this ScenePlaythroughChest chest) => new()
    {
        Id = chest.Id,
        Status = chest.Status,
        RolledValue = chest.RolledValue,
        OpenedAt = chest.OpenedAt,
        SelectedLootEntry = chest.SelectedLootEntry?.ToDto(),
        ScenePlaythrough = chest.ScenePlaythrough.ToDto(),
        SceneChest = chest.SceneChest.ToDto()
    };
}
