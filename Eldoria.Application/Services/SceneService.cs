using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Threading.Tasks.Dataflow;

namespace Eldoria.Application.Services
{
    public class SceneService : ISceneService
    {
        private readonly IAzureStorageBlob _azureStorageBlob;
        private readonly ISceneRepository _sceneRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IJourneyCharacterRepository _journeyCharacterRepository;

        public SceneService(IAzureStorageBlob azureStorageBlob, ISceneRepository sceneRepository, IJourneyRepository journeyRepository, IJourneyCharacterRepository journeyCharacterRepository)
        {
            _azureStorageBlob = azureStorageBlob;
            _sceneRepository = sceneRepository;
            _journeyRepository = journeyRepository;
            _journeyCharacterRepository = journeyCharacterRepository;
        }

        public async Task<Result<SceneDto>> CreateAsync(int userId, int journeyId, string name, string description, IFormFile photo, string gridUrl, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(journeyId);

            if (journey is null)
                return Result<SceneDto>.Fail(new Error("Journey.NotFound", "The associated journey does not exist."));

            if (journey.UserId != userId)
                return Result<SceneDto>.Fail(new Error("Auth.Forbidden", "You do have the permission to this journey you are trying to add a scene to."));


            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

            var scene = new Scene
            {
                JourneyId = journeyId,
                Name = name,
                Description = description,
                PhotoUrl = photoUrl,
                FileName = fileName,
                GridUrl = gridUrl,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            await _sceneRepository.AddAsync(scene, ct);
            await _sceneRepository.SaveChangesAsync(ct);

            return Result<SceneDto>.Ok(scene.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int id, int journeyId, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(journeyId, ct);

            if (journey is null)
                return Result.Fail(new Error("Journey.NotFound", "The associated journey does not exist."));

            if (journey.UserId != userId)
                return Result.Fail(new Error("Auth.Forbidden", "You do have the permission to this journey you are trying to add a scene to."));

            var scene = await _sceneRepository.GetByIdAsync(id, ct);

            if(scene is null)
                return Result.Fail(new Error("Scene.NotFound", "The scene does not exist."));

            await _azureStorageBlob.DeletePhotoFromUrl(scene.PhotoUrl);

            _sceneRepository.Remove(scene);
            await _sceneRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result<SceneDto>> GetByIdAsync(int userId, int id, CancellationToken ct)
        {
            var scene = await _sceneRepository.GetByIdAsync(id, ct);

            if(scene is null)
                return Result<SceneDto>.Fail(new Error("Scene.NotFound", "The scene does not exist."));

            return Result<SceneDto>.Ok(scene.ToDto());
        }

        public async Task<Result<List<SceneDto>>> GetListAsync(int userId, int journeyId, int skip, int take, CancellationToken ct)
        {
            var scenes = await _sceneRepository.GetJourneyScenes(journeyId, skip, take, ct);
            var dtos = scenes.Select(s => s.ToDto()).ToList();

            return Result<List<SceneDto>>.Ok(dtos);            
        }

        public async Task<Result<SceneDashboardDto>> GetSceneDashboardAsync(int userId, int id, int journeyId, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(journeyId, ct);

            if(journey is null)
                return Result<SceneDashboardDto>.Fail(new Error("Journey.NotFound", "The associated journey does not exist."));

            if(journey.UserId != userId)
                return Result<SceneDashboardDto>.Fail(new Error("Auth.Forbidden", "You do have the permission to this journey."));

            var scene = await _sceneRepository.GetSceneDetails(id, ct);

            if(scene is null)
                return Result<SceneDashboardDto>.Fail(new Error("Scene.NotFound", "Scene does not exist."));

            var journeyCharacters = await _journeyCharacterRepository.GetJourneyCharacters(journeyId, ct);

            var journeyCharactersDtos = journeyCharacters.Select(jc => jc.ToDto()).ToList();
            var sceneDto = scene.ToDto();

            return Result<SceneDashboardDto>.Ok(new SceneDashboardDto
            {
                Scene = sceneDto,
                Players = journeyCharactersDtos
            });
        }

        public async Task<Result<SceneDto>> UpdateAsync(int userId, int journeyId, int id, string name, string description, IFormFile? photo, string gridUrl, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(journeyId, ct);

            if (journey is null)
                return Result<SceneDto>.Fail(new Error("Journey.NotFound", "The associated journey does not exist."));

            if (journey.UserId != userId)
                return Result<SceneDto>.Fail(new Error("Auth.Forbidden", "You do not have permission to update this scene."));

            var scene = await _sceneRepository.GetByIdAsync(id, ct);

            if (scene is null)
                return Result<SceneDto>.Fail(new Error("Scene.NotFound", "The scene does not exists."));

            scene.Name = name;
            scene.Description = description;
            scene.GridUrl = gridUrl;
            scene.UpdateDate = DateTime.UtcNow;

            if (photo is not null)
            {
                if (!string.IsNullOrEmpty(journey.FileName))
                    await _azureStorageBlob.DeletePhotoFromUrl(scene.FileName);

                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

                scene.PhotoUrl = photoUrl;
                scene.FileName = fileName;
            }

            _sceneRepository.Update(scene);
            await _sceneRepository.SaveChangesAsync(ct);

            return Result<SceneDto>.Ok(scene.ToDto());
        }
    }
}
