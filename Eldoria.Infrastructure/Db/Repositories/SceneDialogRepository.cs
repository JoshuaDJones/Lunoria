using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneDialogRepository(ApplicationDbContext dbContext)
        : Repository<SceneDialog>(dbContext), ISceneDialogRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        private IQueryable<SceneDialog> QueryForUser(int userId) =>
            _dbContext.SceneDialogs.Where(dialog => dialog.Scene.Journey.UserId == userId);

        public Task<SceneDialog?> GetForUserAsync(int userId, int sceneDialogId, CancellationToken ct) =>
            QueryForUser(userId).SingleOrDefaultAsync(dialog => dialog.Id == sceneDialogId, ct);

        public async Task<List<SceneDialog>> GetSceneDialogs(int userId, int sceneId, CancellationToken ct)
        {
            return await QueryForUser(userId)
                .AsNoTracking()
                .Include(d => d.DialogPages)
                .Include(d => d.DialogPages)
                    .ThenInclude(p => p.DialogPageSections)
                .Include(sd => sd.DialogPages)
                    .ThenInclude(p => p.DialogPageSections)
                        .ThenInclude(s => s.Character)
                .Include(sd => sd.DialogPages)
                    .ThenInclude(p => p.DialogPageSections)
                        .ThenInclude(s => s.Character)
                            .ThenInclude(s => s.CharacterDialogSettings)
                .Where(d => d.SceneId == sceneId)
                .ToListAsync(ct);
        }
    }
}
