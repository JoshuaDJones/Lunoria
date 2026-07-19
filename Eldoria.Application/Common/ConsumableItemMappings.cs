using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class ConsumableItemMappings
    {
        public static ConsumableItemDto ToDto(this ConsumableItem item)
        {
            return new ConsumableItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                PhotoUrl = item.PhotoUrl,
                HpEffect = item.HpEffect,
                MpEffect = item.MpEffect,
                CreatedAt = item.CreatedAt,
            };
        }
    }
}
