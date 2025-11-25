using Eldoria.BlazorClient.Dtos;

namespace Eldoria.BlazorClient.Services.Interfaces
{
    public interface ICharacterService
    {
        Task<List<CharacterDto>> GetCharacterListAsync();
    }
}
