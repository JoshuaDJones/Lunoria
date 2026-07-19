using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterEquippableItemMappings
{
    public static ScenePlaythroughCharacterEquippableItemDto ToDto(
        this ScenePlaythroughCharacterEquippableItem item) => new()
    {
        Id = item.Id,
        EquippableItem = item.EquippableItem.ToDto(),
        ScenePlaythroughCharacter = item.ScenePlaythroughCharacter.ToDto()
    };
}
