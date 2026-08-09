using System.Text.RegularExpressions;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public partial class SceneGridService(
        ISceneGridRepository gridRepository,
        IOwnershipRepository ownershipRepository,
        IAzureStorageBlob azureStorageBlob) : ISceneGridService
    {
        private readonly ISceneGridRepository _gridRepository = gridRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<SceneGridDto>> GetAsync(
            int userId,
            int sceneId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetSceneAsync(userId, sceneId, ct) is null)
                return Result<SceneGridDto>.Fail(SceneNotFound);

            var grid = await _gridRepository.GetForSceneAsync(sceneId, ct);
            return grid is null
                ? Result<SceneGridDto>.Fail(GridNotFound)
                : Result<SceneGridDto>.Ok(grid.ToDto());
        }

        public async Task<Result<SceneGridDto>> CreateAsync(
            int userId,
            int sceneId,
            int rows,
            int columns,
            string gridColor,
            IFormFile? background,
            CancellationToken ct)
        {
            var scene = await _ownershipRepository.GetSceneAsync(userId, sceneId, ct);
            if (scene is null)
                return Result<SceneGridDto>.Fail(SceneNotFound);

            if (await _gridRepository.GetForSceneAsync(sceneId, ct) is not null)
                return Result<SceneGridDto>.Fail(new Error(
                    "SceneGrid.AlreadyExists",
                    "This scene already has an internal grid."));

            var validationError = Validate(rows, columns, gridColor);
            if (validationError is not null)
                return Result<SceneGridDto>.Fail(validationError);

            string? backgroundUrl = null;
            string? backgroundFileName = null;
            if (background is not null)
                (backgroundUrl, backgroundFileName) = await _azureStorageBlob.UploadPhoto(background);

            var now = DateTime.UtcNow;
            var grid = new SceneGrid
            {
                SceneId = sceneId,
                Rows = rows,
                Columns = columns,
                GridColor = gridColor.ToLowerInvariant(),
                BackgroundImageUrl = backgroundUrl,
                BackgroundFileName = backgroundFileName,
                CreatedAt = now,
                UpdatedAt = now,
            };

            scene.GridUrl = null;

            try
            {
                await _gridRepository.AddAsync(grid, ct);
                await _gridRepository.SaveChangesAsync(ct);
            }
            catch
            {
                if (backgroundUrl is not null)
                    await _azureStorageBlob.DeletePhotoFromUrl(backgroundUrl);
                throw;
            }

            return Result<SceneGridDto>.Ok(grid.ToDto());
        }

        public async Task<Result<SceneGridDto>> UpdateAsync(
            int userId,
            int sceneId,
            int rows,
            int columns,
            string gridColor,
            IFormFile? background,
            bool removeBackground,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetSceneAsync(userId, sceneId, ct) is null)
                return Result<SceneGridDto>.Fail(SceneNotFound);

            var grid = await _gridRepository.GetForSceneAsync(sceneId, ct);
            if (grid is null)
                return Result<SceneGridDto>.Fail(GridNotFound);

            var validationError = Validate(rows, columns, gridColor);
            if (validationError is not null)
                return Result<SceneGridDto>.Fail(validationError);

            string? newBackgroundUrl = null;
            string? newBackgroundFileName = null;
            if (background is not null)
                (newBackgroundUrl, newBackgroundFileName) = await _azureStorageBlob.UploadPhoto(background);

            var previousBackgroundUrl = grid.BackgroundImageUrl;
            grid.Rows = rows;
            grid.Columns = columns;
            grid.GridColor = gridColor.ToLowerInvariant();
            grid.UpdatedAt = DateTime.UtcNow;

            if (newBackgroundUrl is not null)
            {
                grid.BackgroundImageUrl = newBackgroundUrl;
                grid.BackgroundFileName = newBackgroundFileName;
            }
            else if (removeBackground)
            {
                grid.BackgroundImageUrl = null;
                grid.BackgroundFileName = null;
            }

            try
            {
                await _gridRepository.SaveChangesAsync(ct);
            }
            catch
            {
                if (newBackgroundUrl is not null)
                    await _azureStorageBlob.DeletePhotoFromUrl(newBackgroundUrl);
                throw;
            }

            if ((newBackgroundUrl is not null || removeBackground) && previousBackgroundUrl is not null)
                await _azureStorageBlob.DeletePhotoFromUrl(previousBackgroundUrl);

            return Result<SceneGridDto>.Ok(grid.ToDto());
        }

        public async Task<Result> DeleteAsync(
            int userId,
            int sceneId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetSceneAsync(userId, sceneId, ct) is null)
                return Result.Fail(SceneNotFound);

            var grid = await _gridRepository.GetForSceneAsync(sceneId, ct);
            if (grid is null)
                return Result.Fail(GridNotFound);

            _gridRepository.Remove(grid);
            await _gridRepository.SaveChangesAsync(ct);

            if (grid.BackgroundImageUrl is not null)
                await _azureStorageBlob.DeletePhotoFromUrl(grid.BackgroundImageUrl);

            return Result.Ok();
        }

        private static Error? Validate(int rows, int columns, string gridColor)
        {
            if (rows is < 1 or > 100 || columns is < 1 or > 100)
                return new Error(
                    "SceneGrid.InvalidDimensions",
                    "Grid rows and columns must each be between 1 and 100.");

            return !HexColorRegex().IsMatch(gridColor)
                ? new Error("SceneGrid.InvalidColor", "Grid color must be a hexadecimal color.")
                : null;
        }

        private static readonly Error SceneNotFound =
            new("Scene.NotFound", "The scene does not exist.");

        private static readonly Error GridNotFound =
            new("SceneGrid.NotFound", "The scene grid does not exist.");

        [GeneratedRegex("^#[0-9a-fA-F]{6}$")]
        private static partial Regex HexColorRegex();
    }
}
