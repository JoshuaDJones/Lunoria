using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface IJourneyService
    {
        Task<Result<List<JourneyDto>>> GetListAsync(int userId, int skip, int take, CancellationToken ct);
        Task<Result<JourneyDto>> GetByIdAsync(int userId, int id, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int id, CancellationToken ct);
        Task<Result<JourneyDto>> CreateAsync(int userId, string name, string description, IFormFile photo, CancellationToken ct);
        Task<Result<JourneyDto>> UpdateAsync(int id, int userId, string name, string description, IFormFile? photo, CancellationToken ct);
    }
}
