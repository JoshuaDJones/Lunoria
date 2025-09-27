using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class ItemMappings
    {
        public static ItemDto ToDto(this Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                PhotoUrl = item.PhotoUrl,
                HpEffect = item.HpEffect,
                MpEffect = item.MpEffect,
                CreateDate = item.CreateDate,
            };
        }
    }
}
