using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneGridRepository : IRepository<SceneGrid>
    {
        Task<SceneGrid?> GetForSceneAsync(int sceneId, CancellationToken ct);
    }
}
