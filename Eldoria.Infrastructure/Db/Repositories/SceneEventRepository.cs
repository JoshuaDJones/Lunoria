using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneEventRepository(ApplicationDbContext dbContext)
        : Repository<SceneEvent>(dbContext), ISceneEventRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        private IQueryable<SceneEvent> Query() =>
            dbContext.SceneEvents
                .Include(sceneEvent => sceneEvent.SceneEventActions)
                .ThenInclude(action => action.CharacterStatAdjustmentAction);

        public Task<List<SceneEvent>> ListForSceneAsync(
            int userId,
            int sceneId,
            CancellationToken ct) =>
            Query()
                .AsNoTracking()
                .Where(sceneEvent =>
                    sceneEvent.SceneId == sceneId &&
                    sceneEvent.Scene.Journey.UserId == userId)
                .OrderBy(sceneEvent => sceneEvent.SortOrder)
                .ToListAsync(ct);

        public Task<SceneEvent?> GetForUserAsync(int userId, int sceneEventId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(sceneEvent => sceneEvent.Id == sceneEventId && sceneEvent.Scene.Journey.UserId == userId, ct);

        public async Task AddWithNextSortOrderAsync(SceneEvent sceneEvent, CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var highestSortOrder = await _dbContext.SceneEvents
                .Where(existing => existing.SceneId == sceneEvent.SceneId)
                .Select(existing => (int?)existing.SortOrder)
                .MaxAsync(ct);

            sceneEvent.SortOrder = (highestSortOrder ?? -1) + 1;

            await _dbContext.SceneEvents.AddAsync(sceneEvent, ct);
            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        public Task<SceneEventAction?> GetActionForUserAsync(int userId, int actionId, CancellationToken ct) =>
            _dbContext.SceneEventActions
                .Include(action => action.CharacterStatAdjustmentAction)
                .SingleOrDefaultAsync(action => action.Id == actionId && action.SceneEvent.Scene.Journey.UserId == userId, ct);

        public async Task AddActionWithNextSortOrderAsync(SceneEventAction action, CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var highestSortOrder = await _dbContext.SceneEventActions
                .Where(existing => existing.SceneEventId == action.SceneEventId)
                .Select(existing => (int?)existing.SortOrder)
                .MaxAsync(ct);

            action.SortOrder = (highestSortOrder ?? -1) + 1;

            await _dbContext.SceneEventActions.AddAsync(action, ct);
            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        public async Task<bool> ReorderAsync(int sceneId, IReadOnlyDictionary<int, int> sortOrders, CancellationToken ct) =>
            await ReorderAsync(_dbContext.SceneEvents, item => item.SceneId == sceneId, item => item.Id, item => item.SortOrder, (item, order) => item.SortOrder = order, sortOrders, ct);

        public async Task<bool> ReorderActionsAsync(int sceneEventId, IReadOnlyDictionary<int, int> sortOrders, CancellationToken ct) =>
            await ReorderAsync(_dbContext.SceneEventActions, item => item.SceneEventId == sceneEventId, item => item.Id, item => item.SortOrder, (item, order) => item.SortOrder = order, sortOrders, ct);

        private async Task<bool> ReorderAsync<T>(
            DbSet<T> set,
            System.Linq.Expressions.Expression<Func<T, bool>> predicate,
            Func<T, int> getId,
            Func<T, int> getSortOrder,
            Action<T, int> setSortOrder,
            IReadOnlyDictionary<int, int> sortOrders,
            CancellationToken ct) where T : class
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var items = await set.Where(predicate).ToListAsync(ct);

            if (items.Count != sortOrders.Count || items.Any(item => !sortOrders.ContainsKey(getId(item))))
                return false;

            var temporaryOffset = items.Count + (items.Count == 0 ? 0 : items.Max(getSortOrder)) + 1;
            foreach (var item in items)
                setSortOrder(item, getSortOrder(item) + temporaryOffset);

            await _dbContext.SaveChangesAsync(ct);

            foreach (var item in items)
                setSortOrder(item, sortOrders[getId(item)]);

            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return true;
        }
    }
}
