using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
