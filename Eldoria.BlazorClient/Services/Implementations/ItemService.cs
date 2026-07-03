using Eldoria.BlazorClient.Dtos;
using Eldoria.BlazorClient.Services.Interfaces;
using System.Net.Http.Json;

namespace Eldoria.BlazorClient.Services.Implementations
{
    public class ItemService(IHttpClientFactory httpClientFactory) : IItemService
    {
        private readonly HttpClient _authClient = httpClientFactory.CreateClient("AuthClient");

        public async Task<List<ItemDto>> GetItemsListAsync()
        {
            var items= await _authClient.GetFromJsonAsync<List<ItemDto>>("item");
            return items ?? [];
        }
    }
}
