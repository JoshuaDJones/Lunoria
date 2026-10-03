using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class SceneObjectiveRepository(ApplicationDbContext db) : ISceneObjectiveRepository
{
    public Task<Scene?> GetTemplateAsync(int userId, int sceneId, CancellationToken ct) =>
        db.Scenes.Include(s => s.Objectives).ThenInclude(o => o.Points).AsSplitQuery()
            .SingleOrDefaultAsync(s => s.Id == sceneId && s.Journey.UserId == userId, ct);

    public Task<ScenePT?> GetRuntimeAsync(int userId, int playthroughId, int sceneId, CancellationToken ct) =>
        db.ScenePTs.Include(s => s.Objectives).ThenInclude(o => o.Points).AsSplitQuery()
            .SingleOrDefaultAsync(s => s.Id == sceneId && s.PlaythroughId == playthroughId
                && s.Playthrough.UserId == userId, ct);

    public async Task SaveChangesAsync(CancellationToken ct) => await db.SaveChangesAsync(ct);
}
