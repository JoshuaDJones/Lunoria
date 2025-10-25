using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneDialogService : ISceneDialogService
    {
        private readonly ISceneDialogRepository _sceneDialogRepository;
        private readonly IRepository<Scene> _sceneRepository;

        public SceneDialogService(ISceneDialogRepository sceneDialogRepository, IRepository<Scene> sceneRepository)
        {
            _sceneDialogRepository = sceneDialogRepository;
            _sceneRepository = sceneRepository;
        }

        public async Task<Result> CreateSceneDialogAsync(int sceneId, string sceneDialogTitle, CancellationToken ct)
        {
            var scene = await _sceneRepository.GetByIdAsync(sceneId, ct);

            if (scene is null)
                return Result.Fail(new Error("Scene.NotFound", "Scene was not found."));

            var newSceneDialog = new SceneDialog
            {
                SceneId = sceneId,
                Title = sceneDialogTitle,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _sceneDialogRepository.AddAsync(newSceneDialog, ct);
            await _sceneDialogRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> DeleteSceneDialogAsync(int sceneDialogId, CancellationToken ct)
        {
            var sceneDialog = await _sceneDialogRepository.GetByIdAsync(sceneDialogId, ct);

            if (sceneDialog is null)
                return Result.Fail(new Error("SceneDialog.NotFound", "Scene dialog does not exist."));

            _sceneDialogRepository.Remove(sceneDialog);
            await _sceneDialogRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> EditSceneDialogAsync(int sceneDialogId, string title, CancellationToken ct)
        {
            var sceneDialog = await _sceneDialogRepository.GetByIdAsync(sceneDialogId, ct);

            if (sceneDialog is null)
                return Result.Fail(new Error("SceneDialog.NotFound", "Scene dialog does not exist."));

            sceneDialog.Title = title;
            sceneDialog.UpdateDate = DateTime.UtcNow;

            await _sceneDialogRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result<List<SceneDialogDto>>> GetSceneDialogsAsync(int sceneId, CancellationToken ct)
        {
            var scene = await _sceneRepository.GetByIdAsync(sceneId, ct);

            if (scene is null)
                return Result<List<SceneDialogDto>>.Fail(new Error("Scene.NotFound", "Scene was not found."));

            var sceneDialogs = await _sceneDialogRepository.GetSceneDialogs(sceneId, ct);
            var dtos = sceneDialogs.Select(d => d.ToDto()).ToList();

            return Result<List<SceneDialogDto>>.Ok(dtos);
        }
    }
}
