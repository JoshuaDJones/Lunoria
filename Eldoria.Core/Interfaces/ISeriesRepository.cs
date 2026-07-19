using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISeriesRepository : IRepository<Series>
    {
        Task<List<Series>> ListForUserAsync(int userId, int skip, int take, CancellationToken ct);
        Task<Series?> GetForUserAsync(int userId, int seriesId, CancellationToken ct);
    }
}
