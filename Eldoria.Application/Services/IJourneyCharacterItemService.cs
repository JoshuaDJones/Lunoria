using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterItemService
    {
        Task<Result> AddJourneyCharacterItem(int journeyCharacterId, int itemId, CancellationToken ct);
        Task<Result> UseJourneyCharacterItem(int journeyCharacterItemId, CancellationToken ct);
    }
}
