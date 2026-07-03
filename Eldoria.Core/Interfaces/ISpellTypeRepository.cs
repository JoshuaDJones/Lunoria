using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISpellTypeRepository : IRepository<SpellType>
    {
        Task<List<SpellType>> GetListForUserAsync(int userId, int skip, int take, CancellationToken ct);
        Task<SpellType?> GetByIdForUserAsync(int userId, int id, CancellationToken ct);
        Task<bool> NameExistsForUserAsync(int userId, string name, int? excludedId, CancellationToken ct);
        Task<bool> IsInUseForUserAsync(int userId, int id, CancellationToken ct);
    }
}
