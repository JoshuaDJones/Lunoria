using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class CharacterStatAdjustmentActionMappings
{
    public static CharacterStatAdjustmentActionDto ToDto(this CharacterStatAdjustmentAction action) => new()
    {
        Id = action.Id,
        CharacterStatType = action.CharacterStatType,
        AdjustmentOperation = action.AdjustmentOperation,
        Value = action.Value,
        CharacterId = action.CharacterId,
        Character = action.Character?.ToDto()
    };
}
