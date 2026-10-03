using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services;

public sealed class SceneObjectiveService(ISceneObjectiveRepository repository) : ISceneObjectiveService
{
    public async Task<Result<List<SceneObjectiveDto>>> ListAsync(int userId, int sceneId, CancellationToken ct)
    {
        var scene = await repository.GetTemplateAsync(userId, sceneId, ct);
        if (scene is null) return Result<List<SceneObjectiveDto>>.Fail(new Error("Scene.NotFound", "Scene was not found."));
        return Result<List<SceneObjectiveDto>>.Ok(scene.Objectives.OrderBy(o => o.SortOrder).ThenBy(o => o.Id)
            .Select(o => new SceneObjectiveDto { Id = o.Id, SortOrder = o.SortOrder,
                Points = o.Points.OrderBy(p => p.SortOrder).ThenBy(p => p.Id).Select(p => p.Text).ToList() }).ToList());
    }

    public async Task<Result> ReplaceAsync(int userId, int sceneId, ReplaceSceneObjectivesInput input, CancellationToken ct)
    {
        if (input.Objectives is null || input.Objectives.Count > 50 ||
            input.Objectives.Any(o => o is null || o.Points is null || o.Points.Count is < 1 or > 50 ||
                o.Points.Any(p => string.IsNullOrWhiteSpace(p) || p.Trim().Length > 1000)))
            return Result.Fail(new Error("Objectives.Invalid", "Use up to 50 sets, with 1–50 nonempty points per set (up to 1000 characters each)."));

        var scene = await repository.GetTemplateAsync(userId, sceneId, ct);
        if (scene is null) return Result.Fail(new Error("Scene.NotFound", "Scene was not found."));
        // Replacing the template graph is saved atomically; runtime snapshots are independent.
        scene.Objectives.Clear();
        foreach (var (objective, index) in input.Objectives.Select((o, i) => (o, i)))
            scene.Objectives.Add(new SceneObjective {
                SortOrder = index,
                Points = objective.Points.Select((text, pointIndex) => new SceneObjectivePoint {
                    SortOrder = pointIndex, Text = text.Trim()
                }).ToList()
            });
        await repository.SaveChangesAsync(ct);
        return Result.Ok();
    }

    public async Task<Result> SelectAsync(int userId, int playthroughId, int sceneId, int index, CancellationToken ct)
    {
        var scene = await repository.GetRuntimeAsync(userId, playthroughId, sceneId, ct);
        if (scene is null) return Result.Fail(new Error("ScenePlaythrough.NotFound", "Scene was not found."));
        if (scene.Status != ScenePlaythroughStatus.InProgress)
            return Result.Fail(new Error("Objectives.SceneNotActive", "Objectives can only be changed in an active scene."));
        if (index < 0 || index >= scene.Objectives.Count)
            return Result.Fail(new Error("Objectives.Invalid", "The objective set does not exist."));
        scene.CurrentObjectiveIndex = index;
        await repository.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
