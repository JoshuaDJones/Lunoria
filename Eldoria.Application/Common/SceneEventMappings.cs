using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SceneEventMappings
{
    public static SceneEventDto ToDto(this SceneEvent sceneEvent) => new()
    {
        Id = sceneEvent.Id,
        Name = sceneEvent.Name,
        Description = sceneEvent.Description,
        SortOrder = sceneEvent.SortOrder,
        Scene = sceneEvent.Scene.ToDto(),
        SceneEventActions = [.. sceneEvent.SceneEventActions.Select(action => action.ToDto())]
    };
}
