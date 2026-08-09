using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneChestLootEntryRepository : IRepository<SceneChestLootEntry>
    {
        Task<List<SceneChestLootEntry>> ListForChestAsync(int userId, int sceneChestId, CancellationToken ct);
        Task<SceneChestLootEntry?> GetForUserAsync(int userId, int sceneChestLootEntryId, CancellationToken ct);
    }
}
