using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface IPlaythroughService
{
    Task<Result<List<PlaythroughSummaryDto>>> GetForJourneyAsync(
        int userId,
        int journeyId,
        CancellationToken ct);
}
