using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SpellRepository(ApplicationDbContext dbContext)
        : Repository<Spell>(dbContext), ISpellRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<List<Spell>> GetSpellsByIdsForUserAsync(
            int userId,
            IReadOnlyCollection<int> spellIds,
            CancellationToken ct)
        {
            return _dbContext.Spells
                .Where(spell => spell.UserId == userId && spellIds.Contains(spell.Id))
                .ToListAsync(ct);
        }

        public Task<List<Spell>> GetListForUserAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            return _dbContext.Spells
                .AsNoTracking()
                .Where(spell => spell.UserId == userId)
                .Include(spell => spell.SpellType)
                .OrderBy(spell => spell.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task<Spell?> GetByIdForUserAsync(int userId, int id, CancellationToken ct)
        {
            return _dbContext.Spells
                .Include(spell => spell.SpellType)
                .SingleOrDefaultAsync(
                    spell => spell.Id == id && spell.UserId == userId,
                    ct);
        }
    }
}
