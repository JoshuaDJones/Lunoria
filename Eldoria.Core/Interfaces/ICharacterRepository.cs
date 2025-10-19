using Eldoria.Core.Entities;
using Eldoria.Core.Enums;

namespace Eldoria.Core.Interfaces
{
    public interface ICharacterRepository: IRepository<Character>
    { 
        Task<List<Character>> GetCharacters(int skip, int take, CharacterType typeFilter, CancellationToken ct);
    }
}
