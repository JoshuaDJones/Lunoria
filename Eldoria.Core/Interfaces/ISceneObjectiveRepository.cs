using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Core.Interfaces;

public interface ISceneObjectiveRepository
{
    Task<Scene?> GetTemplateAsync(int userId, int sceneId, CancellationToken ct);
    Task<ScenePT?> GetRuntimeAsync(int userId, int playthroughId, int sceneId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
