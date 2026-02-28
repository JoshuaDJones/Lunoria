using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class IntroPageMappings
    {
        public static IntroPageDto ToDto(this IntroPage introPage)
        {
            return new IntroPageDto
            {
                Id = introPage.Id,
                JourneyId = introPage.JourneyId,
                Order = introPage.Order,
                Type = introPage.Type,
                Config = introPage.Config,
                PreviewPhotoUrl = introPage.PreviewPhotoUrl
            };
        }
    }
}