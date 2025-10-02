using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Application.Common
{
    public static class JourneyCharacterItemMappings
    {
        public static JourneyCharacterItemDto ToDto(this JourneyCharacterItem journeyCharacterItem)
        {
            return new JourneyCharacterItemDto
            {
                Id = journeyCharacterItem.Id,
                JourneyCharacterId = journeyCharacterItem.Id,
                IsUsed = journeyCharacterItem.IsUsed,
                ItemId = journeyCharacterItem.ItemId,
                Item = journeyCharacterItem.Item.ToDto()
            };
        }
    }
}
