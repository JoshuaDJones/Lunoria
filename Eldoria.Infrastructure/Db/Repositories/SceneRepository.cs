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
            return await _dbContext.Scenes
                .AsNoTracking()
                .Where(j => j.JourneyId == journeyId)
                .OrderBy(s => s.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public async Task<Scene?> GetSceneDetails(int sceneId, CancellationToken ct)
        {
            return await _dbContext.Scenes
                .AsNoTracking()
                .AsSplitQuery() // Prevents cartesian explosion in large include graphs

                // Scene Dialogs
                .Include(s => s.SceneDialogs)
                    .ThenInclude(sd => sd.DialogPages)
                        .ThenInclude(p => p.DialogPageSections)
                            .ThenInclude(dps => dps.Character)
                                .ThenInclude(c => c.CharacterDialogSettings) // ✅ FIXED

                // Scene Characters
                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                        .ThenInclude(c => c.CharacterDialogSettings)

                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                        .ThenInclude(c => c.AlternateForm)
                            .ThenInclude(af => af.CharacterDialogSettings)

                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                            .ThenInclude(cs => cs.Spell)

                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.SceneCharacterItems)
                        .ThenInclude(sci => sci.Item)

                .Where(s => s.Id == sceneId)
                .FirstOrDefaultAsync(ct);
        }
    }
}
