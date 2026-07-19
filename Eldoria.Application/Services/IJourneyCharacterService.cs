using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterService
    {
        Task<Result> ReplaceJourneyCharacters(int userId, int journeyId, List<int> characterIds, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int journeyCharacterId, CancellationToken ct);
    }
}
