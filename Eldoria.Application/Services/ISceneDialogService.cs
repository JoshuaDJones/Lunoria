using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface ISceneDialogService
    {
        Task<Result> CreateSceneDialogAsync(int userId, int sceneId, string sceneDialogTitle, CancellationToken ct);
        Task<Result<List<SceneDialogDto>>> GetSceneDialogsAsync(int userId, int sceneId, CancellationToken ct);
        Task<Result> DeleteSceneDialogAsync(int userId, int sceneDialogId, CancellationToken ct);
        Task<Result> EditSceneDialogAsync(int userId, int sceneDialogId, string title, CancellationToken ct);
    }
}
