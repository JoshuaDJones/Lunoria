using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Eldoria.Core.Enums;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SeriesRepository(ApplicationDbContext dbContext)
        : Repository<Series>(dbContext), ISeriesRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<Series?> GetSeriesByNameAsync(int userId, string name, CancellationToken ct) =>
            _dbContext.Series.AsNoTracking().SingleOrDefaultAsync(series => series.UserId == userId && series.Name == name, ct);
                

        public Task<List<Series>> ListForUserAsync(int userId, int skip, int take, CancellationToken ct) =>
            _dbContext.Series.AsNoTracking().Where(series => series.UserId == userId)
                .OrderBy(series => series.Name).Skip(skip).Take(take).ToListAsync(ct);

        public Task<Series?> GetForUserAsync(int userId, int seriesId, CancellationToken ct) =>
            _dbContext.Series.Include(series => series.Journeys.Where(journey => journey.UserId == userId))
                .SingleOrDefaultAsync(series => series.Id == seriesId && series.UserId == userId, ct);

        public async Task<SeriesDeletionResult> DeleteForUserAsync(int userId, int seriesId, CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, ct);

            var ownedSeries = _dbContext.Series.Where(series => series.Id == seriesId && series.UserId == userId);
            if (!await ownedSeries.AnyAsync(ct))
                return SeriesDeletionResult.NotFound;

            if (await _dbContext.Journeys.IgnoreQueryFilters().AnyAsync(
                journey => journey.SeriesId == seriesId && journey.UserId != userId, ct))
                return SeriesDeletionResult.OwnershipConflict;

            var deleted = await ownedSeries.ExecuteDeleteAsync(ct);
            await transaction.CommitAsync(ct);
            return deleted == 1 ? SeriesDeletionResult.Deleted : SeriesDeletionResult.NotFound;
        }
    }
}
