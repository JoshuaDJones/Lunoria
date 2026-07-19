using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IItemRepository : IRepository<ConsumableItem>
    {
        Task<List<ConsumableItem>> GetListForUserAsync(int userId, int skip, int take, CancellationToken ct);
        Task<ConsumableItem?> GetByIdForUserAsync(int userId, int id, CancellationToken ct);
    }
}
