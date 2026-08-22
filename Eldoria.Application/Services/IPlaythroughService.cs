using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface IPlaythroughService
{
    Task<Result<PlaythroughCreatedDto>> StartAsync(
        int userId,
        int journeyId,
        CancellationToken ct);

    Task<Result<PlaythroughDetailsDto>> GetAsync(
        int userId,
        int playthroughId,
        CancellationToken ct);

    Task<Result<List<PlaythroughSummaryDto>>> GetForJourneyAsync(
        int userId,
        int journeyId,
        CancellationToken ct);
}
