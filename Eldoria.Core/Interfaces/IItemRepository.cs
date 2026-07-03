using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<List<Item>> GetListForUserAsync(int userId, int skip, int take, CancellationToken ct);
        Task<Item?> GetByIdForUserAsync(int userId, int id, CancellationToken ct);
    }
}
