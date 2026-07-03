using Eldoria.BlazorClient.Dtos;
using Eldoria.BlazorClient.Services.Interfaces;
using System.Net.Http.Json;

namespace Eldoria.BlazorClient.Services.Implementations
{
    public class CharacterService(IHttpClientFactory httpClientFactory) : ICharacterService
    {
        private readonly HttpClient _authClient = httpClientFactory.CreateClient("AuthClient");

        public async Task<List<CharacterDto>> GetCharacterListAsync()
        {
            var characters = await _authClient.GetFromJsonAsync<List<CharacterDto>>("character");
            return characters ?? [];
        }
    }
}
