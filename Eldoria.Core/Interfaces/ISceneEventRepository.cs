using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneEventRepository : IRepository<SceneEvent>
    {
        Task<List<SceneEvent>> ListForSceneAsync(int userId, int sceneId, CancellationToken ct);
        Task<SceneEvent?> GetForUserAsync(int userId, int sceneEventId, CancellationToken ct);
        Task AddWithNextSortOrderAsync(SceneEvent sceneEvent, CancellationToken ct);
        Task<bool> ReorderAsync(int sceneId, IReadOnlyDictionary<int, int> sortOrders, CancellationToken ct);
        Task<SceneEventAction?> GetActionForUserAsync(int userId, int actionId, CancellationToken ct);
        Task AddActionWithNextSortOrderAsync(SceneEventAction action, CancellationToken ct);
        Task<bool> ReorderActionsAsync(int sceneEventId, IReadOnlyDictionary<int, int> sortOrders, CancellationToken ct);
    }
}
