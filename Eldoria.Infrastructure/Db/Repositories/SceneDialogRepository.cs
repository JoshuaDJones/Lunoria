using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneDialogRepository : Repository<SceneDialog>, ISceneDialogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SceneDialogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SceneDialog>> GetSceneDialogs(int sceneId, CancellationToken ct)
        {
            return await _dbContext.SceneDialogs
                .AsNoTracking()
                .Include(d => d.DialogPages)
                .Include(d => d.DialogPages)
                    .ThenInclude(p => p.DialogPageSections)
                .Include(sd => sd.DialogPages)
                    .ThenInclude(p => p.DialogPageSections)
                        .ThenInclude(s => s.Character)
                .Where(d => d.SceneId == sceneId)
                .ToListAsync(ct);
        }
    }
}
