using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyPlaythroughCharacterEquippableItemMappings
    {
        public static JourneyPlaythroughCharacterEquippableItemDto ToDto(
            this JourneyPlaythroughCharacterEquippableItem item)
        {
            return new JourneyPlaythroughCharacterEquippableItemDto
            {
                Id = item.Id,
                IsEquipped = item.IsEquipped,
                JourneyPlaythroughCharacterId = item.JourneyPlaythroughCharacterId,
                SourceEquippableItemId = item.EquippableItemId,
                SnapshotEquipmentKey = item.SnapshotEquipmentKey,
            };
        }
    }
}
