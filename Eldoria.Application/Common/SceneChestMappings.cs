using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SceneChestMappings
{
    public static SceneChestDto ToDto(this SceneChest chest) => new()
    {
        Id = chest.Id,
        Name = chest.Name,
        DieSides = chest.DieSides,
        SceneId = chest.SceneId,
        LootEntries = [.. chest.LootEntries.Select(entry => entry.ToDto())]
    };
}
