using Eldoria.Core.Entities;
using Eldoria.Core.Enums;

namespace Eldoria.Core.Interfaces
{
    public interface ICharacterRepository: IRepository<Character>
    { 
        Task<List<Character>> GetCharactersForUserAsync(int userId, int skip, int take, CharacterType typeFilter, CancellationToken ct);
        Task<Character?> GetByIdForUserAsync(int userId, int id, CancellationToken ct);
    }
}
