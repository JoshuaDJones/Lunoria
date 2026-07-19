using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneChestRepository(ApplicationDbContext dbContext)
        : Repository<SceneChest>(dbContext), ISceneChestRepository
    {
        private IQueryable<SceneChest> Query() => dbContext.SceneChests
            .Include(chest => chest.LootEntries).ThenInclude(entry => entry.ConsumableItem)
            .Include(chest => chest.LootEntries).ThenInclude(entry => entry.EquippableItem);

        public Task<List<SceneChest>> ListForSceneAsync(int userId, int sceneId, CancellationToken ct) =>
            Query().AsNoTracking().Where(chest => chest.SceneId == sceneId && chest.Scene.Journey.UserId == userId)
                .OrderBy(chest => chest.Id).ToListAsync(ct);

        public Task<SceneChest?> GetForUserAsync(int userId, int sceneChestId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(chest => chest.Id == sceneChestId && chest.Scene.Journey.UserId == userId, ct);
    }
}
