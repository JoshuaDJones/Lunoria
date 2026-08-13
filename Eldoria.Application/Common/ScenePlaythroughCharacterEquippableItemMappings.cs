using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterEquippableItemMappings
{
    public static ScenePlaythroughCharacterEquippableItemDto ToDto(
        this ScenePlaythroughCharacterEquippableItem item) => new()
    {
        Id = item.Id,
        IsEquipped = item.IsEquipped,
        ScenePlaythroughCharacterId = item.ScenePlaythroughCharacterId,
        SourceEquippableItemId = item.EquippableItemId,
        SnapshotEquipmentKey = item.SnapshotEquipmentKey
    };
}
