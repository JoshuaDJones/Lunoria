using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterConsumableItemMappings
{
    public static ScenePlaythroughCharacterConsumableItemDto ToDto(
        this ScenePlaythroughCharacterConsumableItem item) => new()
    {
        Id = item.Id,
        IsUsed = item.IsUsed,
        ConsumableItem = item.ConsumableItem.ToDto(),
        ScenePlaythroughCharacter = item.ScenePlaythroughCharacter.ToDto()
    };
}
