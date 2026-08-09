using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneGridRepository(ApplicationDbContext dbContext)
        : Repository<SceneGrid>(dbContext), ISceneGridRepository
    {
        public Task<SceneGrid?> GetForSceneAsync(int sceneId, CancellationToken ct) =>
            dbContext.SceneGrids.SingleOrDefaultAsync(grid => grid.SceneId == sceneId, ct);
    }
}
