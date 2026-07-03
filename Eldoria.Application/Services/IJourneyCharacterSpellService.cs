using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterSpellService
    {
        Task<Result<JourneyCharacterSpellDto>> GrantAsync(
            int userId,
            int journeyCharacterId,
            int spellId,
            CancellationToken ct);

        Task<Result> RemoveAsync(
            int userId,
            int journeyCharacterId,
            int spellId,
            CancellationToken ct);
    }
}
