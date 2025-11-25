using Eldoria.BlazorClient.Dtos;

namespace Eldoria.BlazorClient.Services.Interfaces
{
    public interface IItemService
    {
        Task<List<ItemDto>> GetItemsListAsync();
    }
}
