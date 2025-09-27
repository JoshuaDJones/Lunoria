using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ICharacterRepository: IRepository<Character>
    { 
        Task<List<Character>> GetCharacters(int skip, int take, CancellationToken ct);
    }
}
