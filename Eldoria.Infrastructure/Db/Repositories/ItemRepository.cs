using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class ItemRepository(ApplicationDbContext dbContext)
        : Repository<Item>(dbContext), IItemRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<List<Item>> GetListForUserAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            return _dbContext.Items
                .AsNoTracking()
                .Where(item => item.UserId == userId)
                .OrderBy(item => item.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task<Item?> GetByIdForUserAsync(int userId, int id, CancellationToken ct)
        {
            return _dbContext.Items.SingleOrDefaultAsync(
                item => item.Id == id && item.UserId == userId,
                ct);
        }
    }
}
