using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough;

namespace Eldoria.Application.Common
{
    public static class JourneyPlaythroughCharacterConsumableItemMappings
    {
        public static JourneyPlaythroughCharacterConsumableItemDto ToDto(
    this JourneyPlaythroughCharacterConsumableItem item)
        {
            return new JourneyPlaythroughCharacterConsumableItemDto
            {
                Id = item.Id,
                IsUsed = item.IsUsed,
                JourneyPlaythroughCharacterId = item.JourneyPlaythroughCharacterId,
                SourceConsumableItemId = item.ConsumableItemId,
                SnapshotConsumableKey = item.SnapshotConsumableKey,
            };
        }
    }
}
