using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using DialogPage = Eldoria.Core.Entities.ScenePTDialogPage;
using PlaythroughDialogPage = Eldoria.Core.Entities.Playthrough.Scene.ScenePTDialogPage;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class DialogPageRepository(ApplicationDbContext dbContext)
    : Repository<DialogPage>(dbContext), IDialogPageRepository
{
    private IQueryable<DialogPage> QueryForUser(int userId) =>
        dbContext.Set<DialogPage>()
            .Where(page => page.SceneDialog.Scene.Journey.UserId == userId);

    public Task<DialogPage?> GetForUserAsync(
        int userId,
        int dialogPageId,
        CancellationToken ct = default) =>
        QueryForUser(userId).SingleOrDefaultAsync(page => page.Id == dialogPageId, ct);

    public Task<DialogPage?> GetWithSectionsForUserAsync(
        int userId,
        int dialogPageId,
        CancellationToken ct = default)
    {
        return QueryForUser(userId)
            .Include(page => page.DialogPageSections)
            .SingleOrDefaultAsync(page => page.Id == dialogPageId, ct);
    }

    public async Task<bool> IsMediaReferencedAsync(
        string mediaUrl,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl))
            return false;

        return await dbContext.Set<DialogPage>()
                .AsNoTracking()
                .AnyAsync(page => page.MediaUrl == mediaUrl, ct) ||
            await dbContext.Set<PlaythroughDialogPage>()
                .AsNoTracking()
                .AnyAsync(page => page.MediaUrl == mediaUrl, ct);
    }
}
