using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SpellRepository : Repository<Spell>, ISpellRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SpellRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Spell?>> GetSpellsByIds(List<int> spellIds, CancellationToken ct)
        {
            var spells = await _dbContext.Spells
                .Where(s => spellIds.Contains(s.Id))
                .ToListAsync(ct);

            var result = spellIds
                .Select(id => spells.FirstOrDefault(s => s.Id == id))
                .ToList();

            return result;
        }
    }
}
