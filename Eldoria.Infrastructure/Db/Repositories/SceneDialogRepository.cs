using Eldoria.Core.Entities;
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
            var dialogs = await QueryForUser(userId)
                .AsNoTracking()
                .Include(d => d.DialogPages)
                    .ThenInclude(p => p.DialogPageSections)
                .Where(d => d.SceneId == sceneId)
                .ToListAsync(ct);

            var sections = dialogs.SelectMany(d => d.DialogPages)
                .SelectMany(p => p.DialogPageSections).ToList();

            var characterIds = sections.Where(s => s.CharacterId.HasValue)
                .Select(s => s.CharacterId!.Value).Distinct().ToList();
            if (characterIds.Count == 0)
                return dialogs;

            var characters = await _dbContext.Characters.AsNoTracking()
                .Where(c => c.UserId == userId && characterIds.Contains(c.Id))
                .Include(c => c.CharacterDialogSettings)
                .ToDictionaryAsync(c => c.Id, ct);

            foreach (var section in sections)
                section.Character = section.CharacterId is int id && characters.TryGetValue(id, out var character)
                    ? character
                    : null;

            return dialogs;
        }
    }
}
