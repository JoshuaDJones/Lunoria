using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Models;

namespace Eldoria.Application.Services
{
    public interface IEquippableItemService
    {
        Task<Result<List<EquippableItemDto>>> GetListAsync(int userId, int skip, int take, CancellationToken ct);
        Task<Result<EquippableItemDto>> GetByIdAsync(int userId, int id, CancellationToken ct);
        Task<Result<EquippableItemDto>> CreateAsync(int userId, EquippableItemInput input, CancellationToken ct);
        Task<Result<EquippableItemDto>> UpdateAsync(int userId, int id, EquippableItemInput input, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int id, CancellationToken ct);
    }
}
