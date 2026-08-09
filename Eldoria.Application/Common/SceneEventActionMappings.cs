using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SceneEventActionMappings
{
    public static SceneEventActionDto ToDto(this SceneEventAction action) => new()
    {
        Id = action.Id,
        Name = action.Name,
        SortOrder = action.SortOrder,
        ActionTargetType = action.ActionTargetType,
        EventActionType = action.EventActionType,
        SceneEventId = action.SceneEventId,
        CharacterStatAdjustmentAction = action.CharacterStatAdjustmentAction?.ToDto()
    };
}
