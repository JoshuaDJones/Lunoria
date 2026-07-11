using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISpellRepository : IRepository<Spell>
    {
        Task<List<Spell>> GetSpellsByIdsForUserAsync(int userId, IReadOnlyCollection<int> spellIds, CancellationToken ct);
        Task<List<Spell>> GetListForUserAsync(int userId, int skip, int take, int? spellTypeId, CancellationToken ct);
        Task<Spell?> GetByIdForUserAsync(int userId, int id, CancellationToken ct);
    }
}
