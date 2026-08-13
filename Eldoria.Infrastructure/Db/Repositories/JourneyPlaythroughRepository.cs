using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Eldoria.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyPlaythroughRepository(ApplicationDbContext dbContext)
        : Repository<JourneyPlaythrough>(dbContext), IJourneyPlaythroughRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<JourneyPlaythrough> StartAsync(
            int userId,
            int journeyId,
            JourneyRevision revision,
            JourneyPlaythrough playthrough,
            CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                ct);

            if (await _dbContext.JourneyPlaythroughs.AnyAsync(
                    existing => existing.SourceJourneyId == journeyId && existing.IsActive,
                    ct))
                throw new ActivePlaythroughExistsException();

            var existingRevision = await _dbContext.JourneyRevisions.SingleOrDefaultAsync(
                existing => existing.CreatedByUserId == userId &&
                            existing.SourceJourneyId == journeyId &&
                            existing.ContentHash == revision.ContentHash,
                ct);

            if (existingRevision is null)
            {
                revision.RevisionNumber = (await _dbContext.JourneyRevisions
                    .Where(existing => existing.CreatedByUserId == userId &&
                                       existing.SourceJourneyId == journeyId)
                    .MaxAsync(existing => (int?)existing.RevisionNumber, ct) ?? 0) + 1;
                await _dbContext.JourneyRevisions.AddAsync(revision, ct);
                playthrough.JourneyRevision = revision;
            }
            else
            {
                playthrough.JourneyRevision = existingRevision;
                playthrough.JourneyRevisionId = existingRevision.Id;
            }

            await _dbContext.JourneyPlaythroughs.AddAsync(playthrough, ct);

            try
            {
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return playthrough;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new ActivePlaythroughExistsException(ex);
            }
        }

        public Task<JourneyPlaythrough?> GetActiveForJourneyAsync(
            int userId,
            int journeyId,
            CancellationToken ct)
        {
            return _dbContext.JourneyPlaythroughs
                .Include(playthrough => playthrough.JourneyRevision)
                .SingleOrDefaultAsync(
                    playthrough =>
                        playthrough.SourceJourneyId == journeyId &&
                        playthrough.JourneyRevision.CreatedByUserId == userId &&
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
                .Include(playthrough => playthrough.JourneyRevision)
                .SingleOrDefaultAsync(
                    playthrough =>
                        playthrough.Id == playthroughId &&
                        playthrough.SourceJourneyId == journeyId &&
                        playthrough.JourneyRevision.CreatedByUserId == userId,
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
                .Include(playthrough => playthrough.JourneyRevision)
                .Where(playthrough =>
                    playthrough.SourceJourneyId == journeyId &&
                    playthrough.JourneyRevision.CreatedByUserId == userId &&
                    !playthrough.IsActive)
                .OrderByDescending(playthrough => playthrough.StartedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
