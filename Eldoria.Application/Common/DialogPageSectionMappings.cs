using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class DialogPageSectionMappings
    {
        public static DialogPageSectionDto ToDto(this DialogPageSection dialogPageSection)
        {
            return new DialogPageSectionDto
            {
                Id = dialogPageSection.Id,
                OrderNum = dialogPageSection.OrderNum,
                ReadingText = dialogPageSection.ReadingText,
                IsNarrator = dialogPageSection.IsNarrator,
                Character = dialogPageSection.Character?.ToDto(),
            };
        }
    }
}
