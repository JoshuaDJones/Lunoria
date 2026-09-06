using Eldoria.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services;

public sealed record MediaUploadResult(
    string Url,
    string BlobName,
    string ContentType);

public interface IAzureStorageBlob
{
    Task<(string Url, string BlobName)> UploadMedia(IFormFile media);

    Task<MediaUploadResult> UploadMedia(
        IFormFile media,
        DialogPageType pageType,
        CancellationToken ct = default);

    Task<bool> DeletePhotoFromUrl(string? url);

    Task<bool> DeleteMediaAsync(
        string blobName,
        CancellationToken ct = default);
}
