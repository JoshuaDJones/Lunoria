using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISpellRepository : IRepository<Spell>
    {
        Task<List<Spell?>> GetSpellsByIds(List<int> spellIds, CancellationToken ct);
    }
}
