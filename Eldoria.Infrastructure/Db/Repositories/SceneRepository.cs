using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneRepository(ApplicationDbContext dbContext)
        : Repository<Scene>(dbContext), ISceneRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        private IQueryable<Scene> QueryForUser(int userId) =>
            _dbContext.Scenes
                .Where(scene => scene.Journey.UserId == userId);

        private IQueryable<Scene> QueryForJourneyUser(int userId, int journeyId, int sceneId) =>
            QueryForUser(userId)
                .Where(scene => scene.Id == sceneId && scene.JourneyId == journeyId);

        // Tracked for update/delete; no unrelated navigation graph is attached.
        public Task<Scene?> GetForJourneyUserAsync(int userId, int journeyId, int sceneId, CancellationToken ct) =>
            QueryForJourneyUser(userId, journeyId, sceneId).SingleOrDefaultAsync(ct);

        public Task<Scene?> GetForUserAsync(int userId, int sceneId, CancellationToken ct) =>
            QueryForUser(userId)
                .AsNoTracking()
                .Include(scene => scene.Grid)
                .SingleOrDefaultAsync(scene => scene.Id == sceneId, ct);

        public async Task AddWithNextSortOrderAsync(Scene scene, CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                ct);

            var highestSortOrder = await _dbContext.Scenes
                .Where(existingScene => existingScene.JourneyId == scene.JourneyId)
                .Select(existingScene => (int?)existingScene.SortOrder)
                .MaxAsync(ct);

            scene.SortOrder = (highestSortOrder ?? -1) + 1;

            await _dbContext.Scenes.AddAsync(scene, ct);
            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        public async Task<List<Scene>> GetJourneyScenes(int journeyId, int? skip, int? take, CancellationToken ct)
        {
            return await _dbContext.Scenes
                .AsNoTracking()
                .Include(scene => scene.Grid)
                .Where(j => j.JourneyId == journeyId)
                .OrderBy(s => s.SortOrder)
                .Skip(skip ?? 0)
                .Take(take ?? int.MaxValue)
                .ToListAsync(ct);
        }

        public async Task<bool> ReorderAsync(
            int journeyId,
            IReadOnlyDictionary<int, int> sortOrders,
            CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                ct);

            var scenes = await _dbContext.Scenes
                .Where(scene => scene.JourneyId == journeyId)
                .ToListAsync(ct);

            if (scenes.Count != sortOrders.Count ||
                scenes.Any(scene => !sortOrders.ContainsKey(scene.Id)))
            {
                return false;
            }

            var temporaryOffset = scenes.Count + scenes.Max(scene => scene.SortOrder) + 1;

            foreach (var scene in scenes)
                scene.SortOrder += temporaryOffset;

            await _dbContext.SaveChangesAsync(ct);

            foreach (var scene in scenes)
                scene.SortOrder = sortOrders[scene.Id];

            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return true;
        }

    }
}
