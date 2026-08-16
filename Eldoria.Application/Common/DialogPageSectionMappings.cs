using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common
{
    public static class DialogPageSectionMappings
    {
        public static DialogPageSectionDto ToDto(this ScenePTDialogSection dialogPageSection)
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
