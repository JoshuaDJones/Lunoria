using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterService
    {
        Task<Result> ReplaceJourneyCharacters(int userId, int journeyId, List<int> characterIds, CancellationToken ct);
        Task<Result> UpdateJourneyCharacter(int userId, int journeyCharacterId, int newHp, int newMp, bool isAlternateForm, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int journeyCharacterId, CancellationToken ct);
    }
}
