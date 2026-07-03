using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface ISpellService
    {
        Task<Result<List<SpellDto>>> GetListAsync(int userId, int skip, int take, CancellationToken ct);
        Task<Result<SpellDto>> GetByIdAsync(int userId, int id, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int id, CancellationToken ct);
        Task<Result<SpellDto>> CreateAsync(int userId, string name, string description, IFormFile photo, int range, bool isRadius, int mpCost, int? damageEffect, int? healthEffect, int? magicEffect, int spellTypeId, CancellationToken ct);
        Task<Result<SpellDto>> UpdateAsync(int userId, int id, string name, string description, IFormFile? photo, int range, bool isRadius, int mpCost, int? damageEffect, int? healthEffect, int? magicEffect, int spellTypeId, CancellationToken ct);
    }
}
