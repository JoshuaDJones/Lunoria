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
            return IsAssignedInternalAsync(userId, id, ct);
        }

        private async Task<bool> IsAssignedInternalAsync(int userId, int id, CancellationToken ct)
        {
            return await _dbContext.JourneyPlaythroughCharacterEquippableItems.AnyAsync(
                       assignment => assignment.EquippableItemId == id && assignment.EquippableItem.UserId == userId, ct)
                || await _dbContext.ScenePlaythroughCharacterEquippableItems.AnyAsync(
                       assignment => assignment.EquippableItemId == id && assignment.EquippableItem.UserId == userId, ct)
                || await _dbContext.SceneChestLootEntries.AnyAsync(
                       entry => entry.EquippableItemId == id && entry.EquippableItem!.UserId == userId, ct);
        }

        private IQueryable<EquippableItem> QueryForUser(int userId)
        {
            return _dbContext.EquippableItems
                .Where(item => item.UserId == userId)
                .Include(item => item.AddedSpells)
                    .ThenInclude(spell => spell.SpellType)
                .Include(item => item.AffectedSpellType);
        }
    }
}
