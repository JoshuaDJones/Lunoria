using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISpellRepository : IRepository<Spell>
    {
        Task<List<Spell?>> GetSpellsByIds(List<int> spellIds, CancellationToken ct);
        Task<List<Spell>> GetSpellsByIdsForUserAsync(int userId, IReadOnlyCollection<int> spellIds, CancellationToken ct);
    }
}
