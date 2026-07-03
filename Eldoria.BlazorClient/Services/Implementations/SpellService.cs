using Eldoria.BlazorClient.Dtos;
using Eldoria.BlazorClient.Services.Interfaces;
using System.Net.Http.Json;

namespace Eldoria.BlazorClient.Services.Implementations
{
    public class SpellService(IHttpClientFactory httpClientFactory) : ISpellService
    {
        private readonly HttpClient _authClient = httpClientFactory.CreateClient("AuthClient");

        public async Task<List<SpellDto>> GetSpellsListAsync()
        {
            var spells = await _authClient.GetFromJsonAsync<List<SpellDto>>("spell");
            return spells ?? [];
        }
    }
}
