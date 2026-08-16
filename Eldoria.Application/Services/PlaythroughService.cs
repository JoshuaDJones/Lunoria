using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services;

public sealed class PlaythroughService(
    IPlaythroughRepository playthroughRepository,
    IJourneyRepository journeyRepository) : IPlaythroughService
{
    public async Task<Result<List<PlaythroughSummaryDto>>> GetForJourneyAsync(
        int userId,
        int journeyId,
        CancellationToken ct)
    {
        var journey = await journeyRepository.GetByIdAsync(journeyId, ct);

        if (journey is null)
        {
            return Result<List<PlaythroughSummaryDto>>.Fail(
                new Error("Journey.NotFound", "Journey was not found."));
        }

        if (journey.UserId != userId)
        {
            return Result<List<PlaythroughSummaryDto>>.Fail(
                new Error("Auth.Forbidden", "You do not have permission to access this journey."));
        }

        var playthroughs = await playthroughRepository.ListForJourneyAsync(
            userId,
            journeyId,
            ct);

        return Result<List<PlaythroughSummaryDto>>.Ok(
            playthroughs.Select(playthrough => playthrough.ToSummaryDto()).ToList());
    }
}
