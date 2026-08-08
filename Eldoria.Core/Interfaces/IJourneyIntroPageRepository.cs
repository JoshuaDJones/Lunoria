using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyIntroPageRepository : IRepository<JourneyIntroPage>
    {
        Task<List<JourneyIntroPage>> ListForJourneyAsync(int journeyId, CancellationToken ct);
        Task AddWithNextSortOrderAsync(JourneyIntroPage page, CancellationToken ct);
        Task<bool> ReorderAsync(int journeyId, IReadOnlyDictionary<int, int> sortOrders, CancellationToken ct);
    }
}
