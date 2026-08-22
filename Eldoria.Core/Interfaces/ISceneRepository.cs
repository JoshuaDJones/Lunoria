using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneRepository : IRepository<Scene>
    {
        Task<List<Scene>> GetJourneyScenes(int journeyId, int? skip, int? take, CancellationToken ct);
        Task<Scene?> GetSceneDetails(int sceneId, CancellationToken ct);
        Task AddWithNextSortOrderAsync(Scene scene, CancellationToken ct);
        Task<bool> ReorderAsync(int journeyId, IReadOnlyDictionary<int, int> sortOrders, CancellationToken ct);
    }
}
