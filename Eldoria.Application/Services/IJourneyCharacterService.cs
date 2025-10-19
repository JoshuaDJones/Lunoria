using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterService
    {
        Task<Result> ReplaceJourneyCharacters(int journeyId, List<int> characterIds, CancellationToken ct);
        Task<Result> AdjustCharacterHpMpAsync(int journeyCharacterId, int newHp, int newMp, CancellationToken ct);
        Task<Result> DeleteAsync(int  journeyCharacterId, CancellationToken ct);
    }
}
