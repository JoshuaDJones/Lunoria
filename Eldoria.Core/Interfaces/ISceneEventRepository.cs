using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneEventRepository : IRepository<SceneEvent>
    {
        Task<List<SceneEvent>> ListForSceneAsync(int userId, int sceneId, CancellationToken ct);
        Task<SceneEvent?> GetForUserAsync(int userId, int sceneEventId, CancellationToken ct);
    }
}
