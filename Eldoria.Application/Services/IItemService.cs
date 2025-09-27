using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface IItemService
    {
        Task<Result<List<ItemDto>>> GetListAsync(int skip, int take, CancellationToken ct);
        Task<Result<ItemDto>> GetByIdAsync(int id, CancellationToken ct);
        Task<Result> DeleteAsync(int id, CancellationToken ct);
        Task<Result<ItemDto>> CreateAsync(string name, string description, IFormFile photo, int hpEffect, int mpEffect, CancellationToken ct);
        Task<Result<ItemDto>> UpdateAsync(int id, string name, string description, IFormFile? photo, int hpEffect, int mpEffect, CancellationToken ct);
    }
}
