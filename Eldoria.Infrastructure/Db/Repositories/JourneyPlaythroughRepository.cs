using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyPlaythroughRepository(ApplicationDbContext dbContext)
        : Repository<JourneyPlaythrough>(dbContext), IJourneyPlaythroughRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<JourneyPlaythrough?> GetActiveForJourneyAsync(
            int userId,
            int journeyId,
            CancellationToken ct)
        {
            return _dbContext.JourneyPlaythroughs
                .SingleOrDefaultAsync(
                    playthrough =>
                        playthrough.JourneyId == journeyId &&
                        playthrough.Journey.UserId == userId &&
                        playthrough.IsActive,
                    ct);
        }

        public Task<JourneyPlaythrough?> GetForUserAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            return _dbContext.JourneyPlaythroughs
                .SingleOrDefaultAsync(
                    playthrough =>
                        playthrough.Id == playthroughId &&
                        playthrough.JourneyId == journeyId &&
                        playthrough.Journey.UserId == userId,
                    ct);
        }

        public Task<List<JourneyPlaythrough>> ListForJourneyAsync(
            int userId,
            int journeyId,
            int skip,
            int take,
            CancellationToken ct)
        {
            return _dbContext.JourneyPlaythroughs
                .AsNoTracking()
                .Where(playthrough =>
                    playthrough.JourneyId == journeyId &&
                    playthrough.Journey.UserId == userId &&
                    !playthrough.IsActive)
                .OrderByDescending(playthrough => playthrough.StartedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
