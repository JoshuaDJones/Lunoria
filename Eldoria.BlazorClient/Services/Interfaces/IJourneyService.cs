using Eldoria.BlazorClient.Dtos;

namespace Eldoria.BlazorClient.Services.Interfaces
{
    public interface IJourneyService
    {
        Task<List<JourneyDto>> GetJourneysListAsync();
    }
}
