using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

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
                ConsumableItem = item.ConsumableItem.ToDto(),
                JourneyPlaythroughCharacter = item.JourneyPlaythroughCharacter.ToDto(),
            };
        }
    }
}