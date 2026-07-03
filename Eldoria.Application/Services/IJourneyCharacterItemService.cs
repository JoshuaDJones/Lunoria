using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterItemService
    {
        Task<Result> AddJourneyCharacterItem(int userId, int journeyCharacterId, int itemId, CancellationToken ct);
        Task<Result> UseJourneyCharacterItem(int userId, int journeyCharacterItemId, CancellationToken ct);
    }
}
