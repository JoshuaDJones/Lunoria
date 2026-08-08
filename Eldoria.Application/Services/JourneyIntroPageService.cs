using System.Text.Json;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class JourneyIntroPageService(
        IJourneyIntroPageRepository introPageRepository,
        IJourneyRepository journeyRepository,
        IAzureStorageBlob azureStorageBlob) : IJourneyIntroPageService
    {
        private readonly IJourneyIntroPageRepository _introPageRepository = introPageRepository;
        private readonly IJourneyRepository _journeyRepository = journeyRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<List<JourneyIntroPageDto>>> ListAsync(int userId, int journeyId, CancellationToken ct)
        {
            var ownershipError = await GetOwnershipError(userId, journeyId, ct);
            if (ownershipError is not null)
                return Result<List<JourneyIntroPageDto>>.Fail(ownershipError);

            var pages = await _introPageRepository.ListForJourneyAsync(journeyId, ct);
            return Result<List<JourneyIntroPageDto>>.Ok(pages.Select(page => page.ToDto()).ToList());
        }

        public async Task<Result<JourneyIntroPageDto>> CreateAsync(
            int userId,
            int journeyId,
            IntroPageType type,
            string config,
            IFormFile image,
            CancellationToken ct)
        {
            var validationError = Validate(type, config);
            if (validationError is not null)
                return Result<JourneyIntroPageDto>.Fail(validationError);

            var ownershipError = await GetOwnershipError(userId, journeyId, ct);
            if (ownershipError is not null)
                return Result<JourneyIntroPageDto>.Fail(ownershipError);

            var (imageUrl, _) = await _azureStorageBlob.UploadPhoto(image);
            var page = new JourneyIntroPage
            {
                JourneyId = journeyId,
                Type = type,
                Config = config,
                PreviewPhotoUrl = imageUrl
            };

            try
            {
                await _introPageRepository.AddWithNextSortOrderAsync(page, ct);
                return Result<JourneyIntroPageDto>.Ok(page.ToDto());
            }
            catch
            {
                await _azureStorageBlob.DeletePhotoFromUrl(imageUrl);
                throw;
            }
        }

        public async Task<Result<JourneyIntroPageDto>> UpdateAsync(
            int userId,
            int journeyId,
            int id,
            IntroPageType type,
            string config,
            IFormFile? image,
            CancellationToken ct)
        {
            var validationError = Validate(type, config);
            if (validationError is not null)
                return Result<JourneyIntroPageDto>.Fail(validationError);

            var ownershipError = await GetOwnershipError(userId, journeyId, ct);
            if (ownershipError is not null)
                return Result<JourneyIntroPageDto>.Fail(ownershipError);

            var page = await _introPageRepository.GetByIdAsync(id, ct);
            if (page is null || page.JourneyId != journeyId)
                return Result<JourneyIntroPageDto>.Fail(new Error("JourneyIntroPage.NotFound", "The intro page does not exist."));

            string? newImageUrl = null;
            if (image is not null)
                (newImageUrl, _) = await _azureStorageBlob.UploadPhoto(image);

            var previousImageUrl = page.PreviewPhotoUrl;
            page.Type = type;
            page.Config = config;
            if (newImageUrl is not null)
                page.PreviewPhotoUrl = newImageUrl;

            try
            {
                _introPageRepository.Update(page);
                await _introPageRepository.SaveChangesAsync(ct);
            }
            catch
            {
                if (newImageUrl is not null)
                    await _azureStorageBlob.DeletePhotoFromUrl(newImageUrl);
                throw;
            }

            if (newImageUrl is not null && !string.IsNullOrWhiteSpace(previousImageUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(previousImageUrl);

            return Result<JourneyIntroPageDto>.Ok(page.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int journeyId, int id, CancellationToken ct)
        {
            var ownershipError = await GetOwnershipError(userId, journeyId, ct);
            if (ownershipError is not null)
                return Result.Fail(ownershipError);

            var page = await _introPageRepository.GetByIdAsync(id, ct);
            if (page is null || page.JourneyId != journeyId)
                return Result.Fail(new Error("JourneyIntroPage.NotFound", "The intro page does not exist."));

            _introPageRepository.Remove(page);
            await _introPageRepository.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(page.PreviewPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(page.PreviewPhotoUrl);

            return Result.Ok();
        }

        public async Task<Result> ReorderAsync(
            int userId,
            int journeyId,
            IReadOnlyList<(int PageId, int SortOrder)> pages,
            CancellationToken ct)
        {
            var ownershipError = await GetOwnershipError(userId, journeyId, ct);
            if (ownershipError is not null)
                return Result.Fail(ownershipError);

            if (pages.Count == 0 ||
                pages.Select(page => page.PageId).Distinct().Count() != pages.Count ||
                pages.Select(page => page.SortOrder).OrderBy(order => order)
                    .Where((order, index) => order != index).Any())
                return Result.Fail(new Error("JourneyIntroPage.InvalidOrder", "Page IDs must be unique and sort orders contiguous from zero."));

            var reordered = await _introPageRepository.ReorderAsync(
                journeyId,
                pages.ToDictionary(page => page.PageId, page => page.SortOrder),
                ct);

            return reordered
                ? Result.Ok()
                : Result.Fail(new Error("JourneyIntroPage.InvalidOrder", "The submitted pages do not match this journey."));
        }

        private async Task<Error?> GetOwnershipError(int userId, int journeyId, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(journeyId, ct);
            if (journey is null)
                return new Error("Journey.NotFound", "The journey does not exist.");
            return journey.UserId == userId
                ? null
                : new Error("Auth.Forbidden", "You do not have permission to modify this journey.");
        }

        private static Error? Validate(IntroPageType type, string config)
        {
            if (!Enum.IsDefined(type))
                return new Error("JourneyIntroPage.InvalidType", "The intro page type is invalid.");
            if (config.Length > 100_000)
                return new Error("JourneyIntroPage.ConfigTooLarge", "The intro page content is too large.");

            try
            {
                using var document = JsonDocument.Parse(config);
                if (!document.RootElement.TryGetProperty("version", out var version) ||
                    version.ValueKind != JsonValueKind.Number ||
                    !version.TryGetInt32(out var versionNumber) ||
                    versionNumber != 1)
                    return new Error("JourneyIntroPage.InvalidConfig", "The intro page configuration version is invalid.");
            }
            catch (JsonException)
            {
                return new Error("JourneyIntroPage.InvalidConfig", "The intro page configuration must be valid JSON.");
            }

            return null;
        }
    }
}
