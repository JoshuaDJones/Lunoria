using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface IJourneyPlaythroughService
    {
        Task<Result<JourneyPlaythroughDto>> StartAsync(
            int userId,
            int journeyId,
            CancellationToken ct);

        Task<Result<JourneyPlaythroughDto>> GetActiveAsync(
            int userId,
            int journeyId,
            CancellationToken ct);

        Task<Result<List<JourneyPlaythroughDto>>> ListAsync(
            int userId,
            int journeyId,
            int skip,
            int take,
            CancellationToken ct);

        Task<Result<JourneyPlaythroughDto>> CompleteAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct);

        Task<Result<JourneyPlaythroughDto>> DeactivateAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct);
    }
}
