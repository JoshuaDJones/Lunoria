using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyIntroPageMappings
    {
        public static JourneyIntroPageDto ToDto(this JourneyIntroPage introPage)
        {
            return new JourneyIntroPageDto
            {
                Id = introPage.Id,
                SortOrder = introPage.SortOrder,
                Type = introPage.Type,
                Config = introPage.Config,
                PreviewPhotoUrl = introPage.PreviewPhotoUrl,
                Journey = introPage.Journey.ToDto()
            };
        }
    }
}