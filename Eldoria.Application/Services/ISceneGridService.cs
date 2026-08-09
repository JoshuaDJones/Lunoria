using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface ISceneGridService
    {
        Task<Result<SceneGridDto>> GetAsync(int userId, int sceneId, CancellationToken ct);
        Task<Result<SceneGridDto>> CreateAsync(int userId, int sceneId, int rows, int columns, string gridColor, IFormFile? background, CancellationToken ct);
        Task<Result<SceneGridDto>> UpdateAsync(int userId, int sceneId, int rows, int columns, string gridColor, IFormFile? background, bool removeBackground, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int sceneId, CancellationToken ct);
    }
}
