using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Services
{
    public interface ISceneProgressService
    {
        Task<Result<SceneProgressDto>> CreateOrGetAsync(
            int userId,
            int journeyId,
            int sceneId,
            CancellationToken ct);

        Task<Result<SceneProgressDto>> GetActiveAsync(
            int userId,
            int journeyId,
            int sceneId,
            CancellationToken ct);

        Task<Result<List<SceneProgressDto>>> ListAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct);

        Task<Result<SceneProgressDto>> SetStatusAsync(
            int userId,
            int sceneProgressId,
            SceneProgressStatus status,
            CancellationToken ct);
    }
}
