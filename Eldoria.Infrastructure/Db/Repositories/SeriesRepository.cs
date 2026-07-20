using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SeriesRepository(ApplicationDbContext dbContext)
        : Repository<Series>(dbContext), ISeriesRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<List<Series>> ListForUserAsync(int userId, int skip, int take, CancellationToken ct) =>
            _dbContext.Series.AsNoTracking().Where(series => series.UserId == userId)
                .OrderBy(series => series.Name).Skip(skip).Take(take).ToListAsync(ct);

        public Task<Series?> GetForUserAsync(int userId, int seriesId, CancellationToken ct) =>
            _dbContext.Series.Include(series => series.Journeys)
                .SingleOrDefaultAsync(series => series.Id == seriesId && series.UserId == userId, ct);
    }
}
