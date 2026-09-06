using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using DialogPage = Eldoria.Core.Entities.ScenePTDialogPage;
using PlaythroughDialogPage = Eldoria.Core.Entities.Playthrough.Scene.ScenePTDialogPage;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class DialogPageRepository(ApplicationDbContext dbContext)
    : Repository<DialogPage>(dbContext), IDialogPageRepository
{
    public Task<DialogPage?> GetWithSectionsAsync(
        int dialogPageId,
        CancellationToken ct = default)
    {
        return dbContext.Set<DialogPage>()
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
