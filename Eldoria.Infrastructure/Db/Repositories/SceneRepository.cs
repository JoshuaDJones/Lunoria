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

        public async Task<Scene?> GetSceneDetails(int sceneId, CancellationToken ct)
        {
            return await _dbContext.Scenes
                .AsNoTracking()
                .AsSplitQuery() // Prevents cartesian explosion in large include graphs

                .Include(s => s.Grid)

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
                        .ThenInclude(c => c.BaseAlternateForm)
                            .ThenInclude(af => af.CharacterDialogSettings)

                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                            .ThenInclude(cs => cs.Spell)

                .Include(s => s.SceneCharacters)
                    .ThenInclude(sc => sc.SceneCharacterSpells)
                        .ThenInclude(spell => spell.Spell)

                .Include(s => s.SceneChests)
                    .ThenInclude(chest => chest.LootEntries)

                .Include(s => s.SceneEvents)
                    .ThenInclude(sceneEvent => sceneEvent.SceneEventActions)

                .Where(s => s.Id == sceneId)
                .FirstOrDefaultAsync(ct);
        }
    }
}
