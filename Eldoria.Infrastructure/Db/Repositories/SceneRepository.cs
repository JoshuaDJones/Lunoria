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

        public async Task<Scene?> GetSceneDetails(int sceneId, CancellationToken ct)
        {
            return await _dbContext.Scenes
                .AsNoTracking()
                .Include(s => s.SceneDialogs)
                .Include(s => s.SceneDialogs)
                    .ThenInclude(sd => sd.DialogPages)
                .Include(s => s.SceneDialogs)
                    .ThenInclude(sd => sd.DialogPages)
                        .ThenInclude(p => p.DialogPageSections)
                .Include(s => s.SceneDialogs)
                    .ThenInclude(sd => sd.DialogPages)
                        .ThenInclude(p => p.DialogPageSections)
                            .ThenInclude(s => s.Character)
                .Include(s => s.SceneCharacters)
                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                        .ThenInclude(c => c.AlternateForm)
                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                            .ThenInclude(cs => cs.Spell)
                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.SceneCharacterItems)
                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.SceneCharacterItems)
                        .ThenInclude(sci => sci.Item)
                .Where(s => s.Id == sceneId)
                .FirstOrDefaultAsync(ct);
        }
    }
}
