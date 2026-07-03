using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneProgressService(
        ISceneProgressRepository progressRepository,
        IJourneyPlaythroughRepository playthroughRepository,
        IOwnershipRepository ownershipRepository) : ISceneProgressService
    {
        private readonly ISceneProgressRepository _progressRepository = progressRepository;
        private readonly IJourneyPlaythroughRepository _playthroughRepository =
            playthroughRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;

        public async Task<Result<SceneProgressDto>> CreateOrGetAsync(
            int userId,
            int journeyId,
            int sceneId,
            CancellationToken ct)
        {
            var context = await GetActiveContextAsync(userId, journeyId, sceneId, ct);
            if (!context.Success)
                return Result<SceneProgressDto>.Fail(context.Error);

            var existing = await _progressRepository.GetForSceneAsync(
                userId,
                context.Value!.Id,
                sceneId,
                ct);

            if (existing is not null)
                return Result<SceneProgressDto>.Ok(existing.ToDto());

            var progress = new SceneProgress
            {
                SceneId = sceneId,
                JourneyPlaythroughId = context.Value.Id,
                SceneProgressStatus = SceneProgressStatus.NotStarted,
            };

            await _progressRepository.AddAsync(progress, ct);
            await _progressRepository.SaveChangesAsync(ct);
            return Result<SceneProgressDto>.Ok(progress.ToDto());
        }

        public async Task<Result<SceneProgressDto>> GetActiveAsync(
            int userId,
            int journeyId,
            int sceneId,
            CancellationToken ct)
        {
            var context = await GetActiveContextAsync(userId, journeyId, sceneId, ct);
            if (!context.Success)
                return Result<SceneProgressDto>.Fail(context.Error);

            var progress = await _progressRepository.GetForSceneAsync(
                userId,
                context.Value!.Id,
                sceneId,
                ct);

            return progress is null
                ? NotFound<SceneProgressDto>(
                    "SceneProgress.NotFound",
                    "Scene progress was not found.")
                : Result<SceneProgressDto>.Ok(progress.ToDto());
        }

        public async Task<Result<List<SceneProgressDto>>> ListAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct)
        {
            var playthrough = await _playthroughRepository.GetForUserAsync(
                userId,
                journeyId,
                playthroughId,
                ct);

            if (playthrough is null)
            {
                return NotFound<List<SceneProgressDto>>(
                    "JourneyPlaythrough.NotFound",
                    "Journey playthrough was not found.");
            }

            var progressRecords = await _progressRepository.ListForPlaythroughAsync(
                userId,
                playthroughId,
                ct);

            return Result<List<SceneProgressDto>>.Ok(
                progressRecords.Select(progress => progress.ToDto()).ToList());
        }

        public async Task<Result<SceneProgressDto>> SetStatusAsync(
            int userId,
            int sceneProgressId,
            SceneProgressStatus status,
            CancellationToken ct)
        {
            var progress = await _progressRepository.GetAsync(
                userId,
                sceneProgressId,
                ct);

            if (progress is null)
                return NotFound<SceneProgressDto>("SceneProgress.NotFound", "Scene progress was not found.");

            if (!progress.JourneyPlaythrough.IsActive)
            {
                return Result<SceneProgressDto>.Fail(new Error(
                    "SceneProgress.PlaythroughInactive",
                    "Scene progress cannot be changed after its playthrough is inactive."));
            }

            if (progress.SceneProgressStatus == status)
                return Result<SceneProgressDto>.Ok(progress.ToDto());

            var validTransition =
                progress.SceneProgressStatus == SceneProgressStatus.NotStarted &&
                status == SceneProgressStatus.InProgress ||
                progress.SceneProgressStatus == SceneProgressStatus.InProgress &&
                status == SceneProgressStatus.Completed;

            if (!validTransition)
            {
                return Result<SceneProgressDto>.Fail(new Error(
                    "SceneProgress.InvalidTransition",
                    $"Cannot transition scene progress from {progress.SceneProgressStatus} to {status}."));
            }

            progress.SceneProgressStatus = status;
            await _progressRepository.SaveChangesAsync(ct);
            return Result<SceneProgressDto>.Ok(progress.ToDto());
        }

        private async Task<Result<JourneyPlaythrough>> GetActiveContextAsync(
            int userId,
            int journeyId,
            int sceneId,
            CancellationToken ct)
        {
            var scene = await _ownershipRepository.GetSceneAsync(userId, sceneId, ct);
            if (scene is null || scene.JourneyId != journeyId)
            {
                return NotFound<JourneyPlaythrough>(
                    "Scene.NotFound",
                    "Scene was not found in the journey.");
            }

            var playthrough = await _playthroughRepository.GetActiveForJourneyAsync(
                userId,
                journeyId,
                ct);

            return playthrough is null
                ? NotFound<JourneyPlaythrough>(
                    "JourneyPlaythrough.ActiveNotFound",
                    "The journey does not have an active playthrough.")
                : Result<JourneyPlaythrough>.Ok(playthrough);
        }

        private static Result<T> NotFound<T>(string code, string message) =>
            Result<T>.Fail(new Error(code, message));
    }
}
