using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface ISceneService
    {
        Task<Result<List<SceneDto>>> GetListAsync(int userId, int journeyId, int skip, int take, CancellationToken ct);
        Task<Result<SceneDto>> GetByIdAsync(int userId, int id, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int id, int journeyId, CancellationToken ct);
        Task<Result<SceneDto>> CreateAsync(int userId, int journeyId, string name, string description, IFormFile photo, string gridUrl, CancellationToken ct);
        Task<Result<SceneDto>> UpdateAsync(int userId, int journeyId, int id, string name, string description, IFormFile? photo, string gridUrl, CancellationToken ct);

    }
}