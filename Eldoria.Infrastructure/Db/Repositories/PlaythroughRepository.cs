using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class PlaythroughRepository(ApplicationDbContext dbContext)
    : IPlaythroughRepository
{
    public Task<List<Playthrough>> ListForJourneyAsync(
        int userId,
        int sourceJourneyId,
        CancellationToken ct)
    {
        return dbContext.Playthroughs
            .AsNoTracking()
            .Where(playthrough =>
                playthrough.UserId == userId &&
                playthrough.SourceJourneyId == sourceJourneyId)
            .OrderBy(playthrough => playthrough.CompletedAt != null)
            .ThenByDescending(playthrough => playthrough.StartedAt)
            .ToListAsync(ct);
    }
}
