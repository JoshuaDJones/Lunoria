using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class DialogPageSectionRepository(ApplicationDbContext dbContext)
    : Repository<ScenePTDialogSection>(dbContext), IDialogPageSectionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<ScenePTDialogSection?> GetForUserAsync(int userId, int sectionId, CancellationToken ct) =>
        _dbContext.Set<ScenePTDialogSection>()
            .SingleOrDefaultAsync(section => section.Id == sectionId &&
                section.DialogPage.SceneDialog.Scene.Journey.UserId == userId, ct);
}
