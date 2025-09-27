using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyRepository : Repository<Journey>, IJourneyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JourneyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Journey>> GetUsersJourneys(int userId, int skip, int take, CancellationToken ct)
        {
            var query = _dbContext.Journeys
                .AsNoTracking()
                .Where(j => j.UserId == userId);

            return await query
                .OrderBy(j => j.CreateDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
