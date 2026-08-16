using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterConsumableItemMappings
{
    public static ScenePlaythroughCharacterConsumableItemDto ToDto(
        this ScenePlaythroughCharacterConsumableItem item) => new()
    {
        Id = item.Id,
        IsUsed = item.IsUsed,
        ScenePlaythroughCharacterId = item.ScenePlaythroughCharacterId,
        SourceConsumableItemId = item.ConsumableItemId,
        SnapshotConsumableKey = item.SnapshotConsumableKey
    };
}
