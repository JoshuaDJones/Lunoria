using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class EquippableItemRepository(ApplicationDbContext dbContext)
        : Repository<EquippableItem>(dbContext), IEquippableItemRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<List<EquippableItem>> GetListForUserAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            return QueryForUser(userId)
                .AsNoTracking()
                .OrderBy(item => item.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task<EquippableItem?> GetByIdForUserAsync(
            int userId,
            int id,
            CancellationToken ct)
        {
            return QueryForUser(userId)
                .SingleOrDefaultAsync(item => item.Id == id, ct);
        }

        public Task<bool> IsAssignedAsync(int userId, int id, CancellationToken ct)
        {
            return _dbContext.JourneyCharacterEquippableItems
                .IgnoreQueryFilters()
                .AnyAsync(
                    assignment =>
                        assignment.EquippableItemId == id &&
                        assignment.EquippableItem.UserId == userId,
                ct);
        }

        private IQueryable<EquippableItem> QueryForUser(int userId)
        {
            return _dbContext.EquippableItems
                .Where(item => item.UserId == userId)
                .Include(item => item.AddedSpells)
                .Include(item => item.AffectedSpellType);
        }
    }
}
