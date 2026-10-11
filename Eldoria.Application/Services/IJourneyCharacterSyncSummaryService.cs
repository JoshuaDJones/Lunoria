using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface IJourneyCharacterSyncSummaryService
{
    Task<Result<JourneyCharacterSyncSummaryDto>> GetAsync(int userId, int journeyId, CancellationToken ct);
}
