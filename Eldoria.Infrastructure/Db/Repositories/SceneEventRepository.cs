using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneEventRepository(ApplicationDbContext dbContext)
        : Repository<SceneEvent>(dbContext), ISceneEventRepository
    {
        private IQueryable<SceneEvent> Query() => dbContext.SceneEvents
            .Include(sceneEvent => sceneEvent.SceneEventActions)
            .ThenInclude(action => action.CharacterStatAdjustmentAction);

        public Task<List<SceneEvent>> ListForSceneAsync(int userId, int sceneId, CancellationToken ct) =>
            Query().AsNoTracking().Where(sceneEvent => sceneEvent.SceneId == sceneId && sceneEvent.Scene.Journey.UserId == userId)
                .OrderBy(sceneEvent => sceneEvent.SortOrder).ToListAsync(ct);

        public Task<SceneEvent?> GetForUserAsync(int userId, int sceneEventId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(sceneEvent => sceneEvent.Id == sceneEventId && sceneEvent.Scene.Journey.UserId == userId, ct);
    }
}
