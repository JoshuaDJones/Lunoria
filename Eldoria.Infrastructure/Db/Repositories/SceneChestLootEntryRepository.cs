using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneChestLootEntryRepository(ApplicationDbContext dbContext)
        : Repository<SceneChestLootEntry>(dbContext), ISceneChestLootEntryRepository
    {
        private IQueryable<SceneChestLootEntry> Query() => dbContext.SceneChestLootEntries
            .Include(entry => entry.ConsumableItem)
            .Include(entry => entry.EquippableItem);

        public Task<List<SceneChestLootEntry>> ListForChestAsync(
            int userId,
            int sceneChestId,
            CancellationToken ct) =>
            Query()
                .AsNoTracking()
                .Where(entry =>
                    entry.SceneChestId == sceneChestId &&
                    entry.SceneChest.Scene.Journey.UserId == userId)
                .OrderBy(entry => entry.RollMinimum)
                .ThenBy(entry => entry.Id)
                .ToListAsync(ct);

        public Task<SceneChestLootEntry?> GetForUserAsync(
            int userId,
            int sceneChestLootEntryId,
            CancellationToken ct) =>
            Query().SingleOrDefaultAsync(
                entry =>
                    entry.Id == sceneChestLootEntryId &&
                    entry.SceneChest.Scene.Journey.UserId == userId,
                ct);
    }
}
