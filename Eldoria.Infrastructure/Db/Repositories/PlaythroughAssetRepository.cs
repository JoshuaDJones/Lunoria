using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class PlaythroughAssetRepository(ApplicationDbContext dbContext)
    : IPlaythroughAssetRepository
{
    public Task<bool> IsReferencedByRevisionAsync(string assetReference, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(assetReference))
            return Task.FromResult(false);

        var fileName = Uri.TryCreate(assetReference, UriKind.Absolute, out var uri)
            ? uri.AbsolutePath[(uri.AbsolutePath.LastIndexOf('/') + 1)..]
            : assetReference;
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = assetReference;

        return dbContext.JourneyRevisions.AsNoTracking().AnyAsync(
            revision => revision.SnapshotJson.Contains(assetReference) ||
                        revision.SnapshotJson.Contains(fileName),
            ct);
    }
}
