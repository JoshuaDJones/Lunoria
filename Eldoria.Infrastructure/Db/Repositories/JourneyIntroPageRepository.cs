using System.Data;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyIntroPageRepository(ApplicationDbContext dbContext)
        : Repository<JourneyIntroPage>(dbContext), IJourneyIntroPageRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<List<JourneyIntroPage>> ListForJourneyAsync(int journeyId, CancellationToken ct)
            => _dbContext.JourneyIntroPages
                .AsNoTracking()
                .Where(page => page.JourneyId == journeyId)
                .OrderBy(page => page.SortOrder)
                .ToListAsync(ct);

        public async Task AddWithNextSortOrderAsync(JourneyIntroPage page, CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                ct);

            var highestSortOrder = await _dbContext.JourneyIntroPages
                .Where(existingPage => existingPage.JourneyId == page.JourneyId)
                .Select(existingPage => (int?)existingPage.SortOrder)
                .MaxAsync(ct);

            page.SortOrder = (highestSortOrder ?? -1) + 1;
            await _dbContext.JourneyIntroPages.AddAsync(page, ct);
            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        public async Task<bool> ReorderAsync(
            int journeyId,
            IReadOnlyDictionary<int, int> sortOrders,
            CancellationToken ct)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                ct);

            var pages = await _dbContext.JourneyIntroPages
                .Where(page => page.JourneyId == journeyId)
                .ToListAsync(ct);

            if (pages.Count != sortOrders.Count ||
                pages.Any(page => !sortOrders.ContainsKey(page.Id)))
                return false;

            var temporaryOffset = pages.Count + pages.Max(page => page.SortOrder) + 1;

            foreach (var page in pages)
                page.SortOrder += temporaryOffset;

            await _dbContext.SaveChangesAsync(ct);

            foreach (var page in pages)
                page.SortOrder = sortOrders[page.Id];

            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return true;
        }
    }
}
