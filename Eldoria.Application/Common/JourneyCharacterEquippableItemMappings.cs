using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyCharacterEquippableItemMappings
    {
        public static JourneyCharacterEquippableItemDto ToDto(
            this JourneyCharacterEquippableItem item)
        {
            return new JourneyCharacterEquippableItemDto
            {
                Id = item.Id,
                JourneyCharacterId = item.JourneyCharacterId,
                EquippableItemId = item.EquippableItemId,
                IsEquipped = item.IsEquipped,
                EquippableItem = item.EquippableItem.ToDto(),
            };
        }
    }
}
