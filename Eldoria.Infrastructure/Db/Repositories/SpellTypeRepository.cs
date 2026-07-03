using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SpellTypeRepository(ApplicationDbContext dbContext)
        : Repository<SpellType>(dbContext), ISpellTypeRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<List<SpellType>> GetListForUserAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            return _dbContext.SpellTypes
                .AsNoTracking()
                .Where(spellType => spellType.UserId == userId)
                .OrderBy(spellType => spellType.TypeName)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task<SpellType?> GetByIdForUserAsync(int userId, int id, CancellationToken ct)
        {
            return _dbContext.SpellTypes.SingleOrDefaultAsync(
                spellType => spellType.Id == id && spellType.UserId == userId,
                ct);
        }

        public Task<bool> NameExistsForUserAsync(
            int userId,
            string name,
            int? excludedId,
            CancellationToken ct)
        {
            return _dbContext.SpellTypes.AnyAsync(
                spellType =>
                    spellType.UserId == userId &&
                    spellType.TypeName == name &&
                    (!excludedId.HasValue || spellType.Id != excludedId.Value),
                ct);
        }

        public Task<bool> IsInUseForUserAsync(int userId, int id, CancellationToken ct)
        {
            return _dbContext.SpellTypes.AnyAsync(
                spellType =>
                    spellType.Id == id &&
                    spellType.UserId == userId &&
                    (spellType.Spells.Any() || spellType.AffectedEquippableItems.Any()),
                ct);
        }
    }
}
