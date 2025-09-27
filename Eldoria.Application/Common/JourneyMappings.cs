using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyMappings
    {
        public static JourneyDto ToDto(this Journey journey)
        {
            return new JourneyDto
            {
                Id = journey.Id,
                Name = journey.Name,
                Description = journey.Description,
                PhotoUrl = journey.PhotoUrl,
                CreateDate = journey.CreateDate,
            };
        }
    }
}
