using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface IConsumableItemService
    {
        Task<Result<List<ConsumableItemDto>>> GetListAsync(
            int userId, int skip, int take, CancellationToken ct);

        Task<Result<ConsumableItemDto>> GetByIdAsync(
            int userId, int id, CancellationToken ct);

        Task<Result<ConsumableItemDto>> CreateAsync(
            int userId, string name, string description, IFormFile photo,
            int hpEffect, int mpEffect, CancellationToken ct);

        Task<Result<ConsumableItemDto>> UpdateAsync(
            int userId, int id, string name, string description, IFormFile? photo,
            int hpEffect, int mpEffect, CancellationToken ct);

        Task<Result> DeleteAsync(int userId, int id, CancellationToken ct);
    }
}
