using Eldoria.BlazorClient.Dtos;

namespace Eldoria.BlazorClient.Services.Interfaces
{
    public interface ISpellService
    {
        Task<List<SpellDto>> GetSpellsListAsync();
    }
}
