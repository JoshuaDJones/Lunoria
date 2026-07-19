using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneChestRepository : IRepository<SceneChest>
    {
        Task<List<SceneChest>> ListForSceneAsync(int userId, int sceneId, CancellationToken ct);
        Task<SceneChest?> GetForUserAsync(int userId, int sceneChestId, CancellationToken ct);
    }
}
