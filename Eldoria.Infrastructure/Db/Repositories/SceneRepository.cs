using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneRepository : Repository<Scene>, ISceneRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public SceneRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Scene>> GetJourneyScenes(int journeyId, int skip, int take, CancellationToken ct)
        {
            var query = _dbContext.Scenes
                .AsNoTracking()
                .Where(j => j.JourneyId == journeyId);

            return await query
                .OrderBy(s => s.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
