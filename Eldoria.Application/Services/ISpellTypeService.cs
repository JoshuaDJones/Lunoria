using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface ISpellTypeService
    {
        Task<Result<List<SpellTypeDto>>> GetListAsync(int userId, int skip, int take, CancellationToken ct);
        Task<Result<SpellTypeDto>> GetByIdAsync(int userId, int id, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int id, CancellationToken ct);
        Task<Result<SpellTypeDto>> CreateAsync(int userId, string name, string description, Microsoft.AspNetCore.Http.IFormFile photo, CancellationToken ct);
        Task<Result<SpellTypeDto>> UpdateAsync(int userId, int id, string name, string description, Microsoft.AspNetCore.Http.IFormFile? photo, CancellationToken ct);
    }
}
