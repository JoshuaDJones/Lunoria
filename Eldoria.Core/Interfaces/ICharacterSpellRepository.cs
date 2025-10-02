using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ICharacterSpellRepository: IRepository<CharacterSpell>
    { 
        Task<List<CharacterSpell>> GetCharacterSpells(int characterId, CancellationToken ct);
        Task RemoveCharacterSpells(int characterId, CancellationToken ct);
        Task AddCharacterSpells(List<int> spellIds, int characterId, CancellationToken ct);
    }
}
