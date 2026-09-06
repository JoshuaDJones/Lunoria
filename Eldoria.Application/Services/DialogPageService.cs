using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Eldoria.Application.Services;

public class DialogPageService(
    IDialogPageRepository dialogPageRepository,
    IRepository<SceneDialog> sceneDialogRepository,
    IAzureStorageBlob azureStorageBlob,
    ILogger<DialogPageService> logger) : IDialogPageService
{
    public async Task<Result> CreateDialogPageAsync(
        int sceneDialogId,
        int orderNum,
        DialogPageType pageType,
        IFormFile media,
        CancellationToken ct)
    {
        var sceneDialog = await sceneDialogRepository.GetByIdAsync(
            sceneDialogId,
            ct);

        if (sceneDialog is null)
        {
            return Result.Fail(new Error(
                "SceneDialog.NotFound",
                "Scene dialog does not exist."));
        }

        var uploadResult = await UploadAsync(media, pageType, ct);
        if (!uploadResult.Success)
            return Result.Fail(uploadResult.Error);

        var uploaded = uploadResult.Value!;
        var now = DateTime.UtcNow;
        var dialogPage = new ScenePTDialogPage
        {
            SceneDialogId = sceneDialogId,
            OrderNum = orderNum,
            PageType = pageType,
            MediaUrl = uploaded.Url,
            MediaBlobName = uploaded.BlobName,
            MediaContentType = uploaded.ContentType,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            await dialogPageRepository.AddAsync(dialogPage, ct);
            await dialogPageRepository.SaveChangesAsync(ct);
        }
        catch
        {
            await TryDeleteMediaAsync(uploaded.BlobName);
            throw;
        }

        return Result.Ok();
    }

    public async Task<Result> DeleteDialogPageAsync(
        int dialogPageId,
        CancellationToken ct)
    {
        var dialogPage = await dialogPageRepository.GetByIdAsync(
            dialogPageId,
            ct);

        if (dialogPage is null)
        {
            return Result.Fail(new Error(
                "DialogPage.NotFound",
                "Dialog page does not exist."));
        }

        var oldMediaUrl = dialogPage.MediaUrl;
        var oldBlobName = dialogPage.MediaBlobName;

        dialogPageRepository.Remove(dialogPage);
        await dialogPageRepository.SaveChangesAsync(ct);
        await DeleteIfOrphanedAsync(oldMediaUrl, oldBlobName, ct);

        return Result.Ok();
    }

    public async Task<Result> EditDialogPageAsync(
        int dialogPageId,
        int? orderNum,
        DialogPageType? pageType,
        IFormFile? media,
        CancellationToken ct)
    {
        var dialogPage = await dialogPageRepository.GetWithSectionsAsync(
            dialogPageId,
            ct);

        if (dialogPage is null)
        {
            return Result.Fail(new Error(
                "DialogPage.NotFound",
                "Dialog page does not exist."));
        }

        var targetPageType = pageType ?? dialogPage.PageType;

        if (!Enum.IsDefined(targetPageType))
        {
            return Result.Fail(new Error(
                "DialogPage.InvalidPageType",
                "The dialog page type is invalid."));
        }

        if (targetPageType == DialogPageType.Video &&
            dialogPage.DialogPageSections.Count > 0)
        {
            return Result.Fail(new Error(
                "DialogPage.VideoHasSections",
                "Remove all dialog sections before changing this page to video."));
        }

        if (targetPageType != dialogPage.PageType && media is null)
        {
            return Result.Fail(new Error(
                "DialogPage.MediaRequired",
                "New media is required when changing the page type."));
        }

        MediaUploadResult? uploaded = null;
        if (media is not null)
        {
            var uploadResult = await UploadAsync(media, targetPageType, ct);
            if (!uploadResult.Success)
                return Result.Fail(uploadResult.Error);
            uploaded = uploadResult.Value;
        }

        var oldMediaUrl = dialogPage.MediaUrl;
        var oldBlobName = dialogPage.MediaBlobName;

        if (orderNum.HasValue)
            dialogPage.OrderNum = orderNum.Value;

        dialogPage.PageType = targetPageType;
        dialogPage.UpdatedAt = DateTime.UtcNow;

        if (uploaded is not null)
        {
            dialogPage.MediaUrl = uploaded.Url;
            dialogPage.MediaBlobName = uploaded.BlobName;
            dialogPage.MediaContentType = uploaded.ContentType;
        }

        try
        {
            await dialogPageRepository.SaveChangesAsync(ct);
        }
        catch
        {
            if (uploaded is not null)
                await TryDeleteMediaAsync(uploaded.BlobName);
            throw;
        }

        if (uploaded is not null)
            await DeleteIfOrphanedAsync(oldMediaUrl, oldBlobName, ct);

        return Result.Ok();
    }

    private async Task<Result<MediaUploadResult>> UploadAsync(
        IFormFile media,
        DialogPageType pageType,
        CancellationToken ct)
    {
        try
        {
            var uploaded = await azureStorageBlob.UploadMedia(media, pageType, ct);
            return Result<MediaUploadResult>.Ok(uploaded);
        }
        catch (ArgumentException exception)
        {
            return Result<MediaUploadResult>.Fail(new Error(
                "DialogPage.InvalidMedia",
                exception.Message));
        }
    }

    private async Task DeleteIfOrphanedAsync(
        string mediaUrl,
        string blobName,
        CancellationToken ct)
    {
        try
        {
            if (!await dialogPageRepository.IsMediaReferencedAsync(mediaUrl, ct))
                await TryDeleteMediaAsync(blobName);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Could not complete orphan cleanup for dialog-page media blob {BlobName}.",
                blobName);
        }
    }

    private async Task TryDeleteMediaAsync(string blobName)
    {
        try
        {
            await azureStorageBlob.DeleteMediaAsync(
                blobName,
                CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Could not delete orphaned dialog-page media blob {BlobName}.",
                blobName);
        }
    }
}
