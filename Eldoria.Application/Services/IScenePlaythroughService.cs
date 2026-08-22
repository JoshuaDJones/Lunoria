using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface IScenePlaythroughService
{
    Task<Result> StartAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<Result<ScenePlaythroughDetailsDto>> GetAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<Result> AddSceneCharacterInstanceAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int scenePlaythroughCharacterId,
        CancellationToken ct);
}
