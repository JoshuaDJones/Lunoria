using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

public static class SceneObjectiveMappings
{
    public static List<SceneObjectiveDto> ToObjectiveDtos(this ScenePT scene) =>
        scene.Objectives.OrderBy(o => o.SortOrder).ThenBy(o => o.Id)
            .Select(o => new SceneObjectiveDto {
                Id = o.Id, SortOrder = o.SortOrder,
                Points = o.Points.OrderBy(p => p.SortOrder).ThenBy(p => p.Id).Select(p => p.Text).ToList()
            }).ToList();
}
