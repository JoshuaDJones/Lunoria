using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class DialogPageSectionMappings
    {
        public static DialogPageSectionDto ToDto(this ScenePTDialogSection dialogPageSection, int userId)
        {
            return new DialogPageSectionDto
            {
                Id = dialogPageSection.Id,
                OrderNum = dialogPageSection.OrderNum,
                ReadingText = dialogPageSection.ReadingText,
                IsNarrator = dialogPageSection.IsNarrator,
                Character = dialogPageSection.Character is { } character && character.UserId == userId
                    ? character.ToDto()
                    : null,
            };
        }
    }
}
