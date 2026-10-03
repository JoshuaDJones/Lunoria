using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface ISceneObjectiveService
{
    Task<Result<List<SceneObjectiveDto>>> ListAsync(int userId, int sceneId, CancellationToken ct);
    Task<Result> ReplaceAsync(int userId, int sceneId, ReplaceSceneObjectivesInput input, CancellationToken ct);
    Task<Result> SelectAsync(int userId, int playthroughId, int sceneId, int index, CancellationToken ct);
}
