using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class DialogPageMappings
    {
        public static DialogPageDto ToDto(this DialogPage dialogPage)
        {
            return new DialogPageDto
            {
                Id = dialogPage.Id,
                OrderNum = dialogPage.OrderNum,
                PhotoUrl = dialogPage.PhotoUrl,
                DialogPageSections = dialogPage.DialogPageSections.Select(s => s.ToDto()).ToList()
            };
        }
    }
}
