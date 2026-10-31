using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class DialogPageMappings
    {
        public static DialogPageDto ToDto(this ScenePTDialogPage dialogPage, int userId)
        {
            return new DialogPageDto
            {
                Id = dialogPage.Id,
                OrderNum = dialogPage.OrderNum,
                PageType = dialogPage.PageType,
                MediaUrl = dialogPage.MediaUrl,
                MediaContentType = dialogPage.MediaContentType,
                DialogPageSections = [.. dialogPage.DialogPageSections.Select(s => s.ToDto(userId))]
            };
        }
    }
}
