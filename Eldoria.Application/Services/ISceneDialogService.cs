using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface ISceneDialogService
    {
        Task<Result> CreateSceneDialogAsync(int sceneId, string sceneDialogTitle, CancellationToken ct);
        Task<Result<List<SceneDialogDto>>> GetSceneDialogsAsync(int sceneId, CancellationToken ct);
    }
}
