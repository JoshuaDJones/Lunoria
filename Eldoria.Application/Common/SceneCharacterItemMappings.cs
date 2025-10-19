using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Application.Common
{
    public static class SceneCharacterItemMappings
    {
        public static SceneCharacterItemDto ToDto(this SceneCharacterItem sceneCharacterItem)
        {
            return new SceneCharacterItemDto
            {
                Id = sceneCharacterItem.Id,
                IsUsed = sceneCharacterItem.IsUsed,
                SceneCharacterId = sceneCharacterItem.SceneCharacterId,
                ItemId = sceneCharacterItem.ItemId,
                Item = sceneCharacterItem.Item.ToDto()
            };
        }
    }
}
