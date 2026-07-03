using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IEquippableItemRepository : IRepository<EquippableItem>
    {
        Task<List<EquippableItem>> GetListForUserAsync(int userId, int skip, int take, CancellationToken ct);
        Task<EquippableItem?> GetByIdForUserAsync(int userId, int id, CancellationToken ct);
        Task<bool> IsAssignedAsync(int userId, int id, CancellationToken ct);
    }
}
