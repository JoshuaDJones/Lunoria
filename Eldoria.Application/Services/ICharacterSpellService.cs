using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface ICharacterSpellService
    {
        Task<Result> ReplaceCharacterSpells(int userId, int characterId, List<int> spellIds, CancellationToken ct);
    }
}
