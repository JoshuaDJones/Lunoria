using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface ISeriesService
    {
        Task<Result<List<SeriesDto>>> GetListAsync(int userId, int skip, int take, CancellationToken ct);
        Task<Result<SeriesDto>> GetByIdAsync(int userId, int id, CancellationToken ct);
        Task<Result<SeriesDto>> CreateAsync(int userId, string name, string? description, IFormFile? photo, CancellationToken ct);
        Task<Result<SeriesDto>> UpdateAsync(int userId, int id, string name, string? description, IFormFile? photo, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int id, CancellationToken ct);
    }
}
