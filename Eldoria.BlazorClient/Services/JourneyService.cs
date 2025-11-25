using Eldoria.BlazorClient.Dtos;
using Eldoria.BlazorClient.Services.Interfaces;
using System.Net.Http.Json;

namespace Eldoria.BlazorClient.Services
{
    public class JourneyService : IJourneyService
    {
        private readonly HttpClient _authClient;

        public JourneyService(IHttpClientFactory httpClientFactory)
        {
            _authClient = httpClientFactory.CreateClient("AuthClient");
        }

        public async Task<List<JourneyDto>> GetJourneysListAsync()
        {
            var journeys = await _authClient.GetFromJsonAsync<List<JourneyDto>>("journey");
            return journeys ?? [];
        }
    }
}
