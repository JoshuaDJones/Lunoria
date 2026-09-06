using System.Text;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Eldoria.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Eldoria.Application.Services;

public class AzureStorageBlob(IConfiguration config) : IAzureStorageBlob
{
    private const long DefaultMaxImageBytes = 10 * 1024 * 1024;
    private const long DefaultMaxVideoBytes = 100 * 1024 * 1024;
    private const int VideoProbeBytes = 1024 * 1024;

    private readonly string _containerName = config["AzureStorage:ContainerName"] ?? "";
    private readonly BlobServiceClient _blobServiceClient = CreateClient(config);
    private readonly long _maxImageBytes = ReadPositiveLong(
        config["AzureStorage:MaxImageBytes"],
        DefaultMaxImageBytes);
    private readonly long _maxVideoBytes = ReadPositiveLong(
        config["AzureStorage:MaxVideoBytes"],
        DefaultMaxVideoBytes);

    public async Task<(string Url, string BlobName)> UploadMedia(IFormFile media)
    {
        var uploaded = await UploadMedia(media, DialogPageType.Image);
        return (uploaded.Url, uploaded.BlobName);
    }

    public async Task<MediaUploadResult> UploadMedia(
        IFormFile media,
        DialogPageType pageType,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(media);

        var format = await ValidateAsync(media, pageType, ct);

        try
        {
            var containerClient =
                _blobServiceClient.GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync(
                PublicAccessType.Blob,
                cancellationToken: ct);

            var blobName = $"{Guid.NewGuid():N}{format.Extension}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = media.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = format.ContentType
                    }
                },
                ct);

            return new MediaUploadResult(
                blobClient.Uri.ToString(),
                blobName,
                format.ContentType);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Azure status: {ex.Status}");
            Console.WriteLine($"Azure error code: {ex.ErrorCode}");
            Console.WriteLine($"Azure message: {ex.Message}");
            throw;
        }
    }

    public Task<bool> DeletePhotoFromUrl(string? blobUrl)
    {
        // Existing asset types still share URLs with playthrough snapshots.
        return Task.FromResult(true);
    }

    public async Task<bool> DeleteMediaAsync(
        string blobName,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(blobName) ||
            !string.Equals(
                Path.GetFileName(blobName),
                blobName,
                StringComparison.Ordinal))
        {
            return false;
        }

        var containerClient =
            _blobServiceClient.GetBlobContainerClient(_containerName);
        var response = await containerClient
            .GetBlobClient(blobName)
            .DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                cancellationToken: ct);

        return response.Value;
    }

    private async Task<MediaFormat> ValidateAsync(
        IFormFile media,
        DialogPageType pageType,
        CancellationToken ct)
    {
        if (!Enum.IsDefined(pageType))
            throw new ArgumentException("The dialog page type is invalid.", nameof(pageType));

        if (media.Length == 0)
            throw new ArgumentException("The uploaded media is empty.", nameof(media));

        var maximumBytes = pageType == DialogPageType.Video
            ? _maxVideoBytes
            : _maxImageBytes;

        if (media.Length > maximumBytes)
        {
            throw new ArgumentException(
                $"The uploaded {pageType.ToString().ToLowerInvariant()} exceeds the {maximumBytes / 1024 / 1024} MB limit.",
                nameof(media));
        }

        var extension = Path.GetExtension(media.FileName).ToLowerInvariant();
        var format = GetFormat(pageType, extension);

        if (!format.AcceptedContentTypes.Contains(
            media.ContentType,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"The file MIME type '{media.ContentType}' does not match the selected page type.",
                nameof(media));
        }

        var probeLength = (int)Math.Min(
            media.Length,
            pageType == DialogPageType.Video ? VideoProbeBytes : 16);
        var probe = new byte[probeLength];

        await using (var stream = media.OpenReadStream())
        {
            var totalRead = 0;
            while (totalRead < probe.Length)
            {
                var read = await stream.ReadAsync(
                    probe.AsMemory(totalRead, probe.Length - totalRead),
                    ct);
                if (read == 0)
                    break;
                totalRead += read;
            }
        }

        if (!HasValidSignature(format.Extension, probe))
        {
            throw new ArgumentException(
                "The file contents do not match its extension.",
                nameof(media));
        }

        if (pageType == DialogPageType.Video && !ContainsH264Marker(probe))
        {
            throw new ArgumentException(
                "MP4 videos must use H.264 video and fast-start metadata.",
                nameof(media));
        }

        return format;
    }

    private static MediaFormat GetFormat(
        DialogPageType pageType,
        string extension)
    {
        if (pageType == DialogPageType.Video)
        {
            return extension == ".mp4"
                ? new MediaFormat(".mp4", "video/mp4", ["video/mp4"])
                : throw new ArgumentException("Only MP4 videos are supported.");
        }

        return extension switch
        {
            ".jpg" or ".jpeg" => new(
                extension,
                "image/jpeg",
                ["image/jpeg", "image/jpg", "image/pjpeg"]),
            ".png" => new(".png", "image/png", ["image/png"]),
            ".gif" => new(".gif", "image/gif", ["image/gif"]),
            ".webp" => new(".webp", "image/webp", ["image/webp"]),
            _ => throw new ArgumentException(
                "Supported images are JPEG, PNG, GIF, and WebP.")
        };
    }

    private static bool HasValidSignature(
        string extension,
        ReadOnlySpan<byte> bytes)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" =>
                bytes.Length >= 3 &&
                bytes[0] == 0xFF &&
                bytes[1] == 0xD8 &&
                bytes[2] == 0xFF,
            ".png" =>
                bytes.Length >= 8 &&
                bytes[..8].SequenceEqual(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".gif" =>
                bytes.Length >= 6 &&
                (Encoding.ASCII.GetString(bytes[..6]) == "GIF87a" ||
                 Encoding.ASCII.GetString(bytes[..6]) == "GIF89a"),
            ".webp" =>
                bytes.Length >= 12 &&
                Encoding.ASCII.GetString(bytes[..4]) == "RIFF" &&
                Encoding.ASCII.GetString(bytes.Slice(8, 4)) == "WEBP",
            ".mp4" =>
                bytes.Length >= 12 &&
                Encoding.ASCII.GetString(bytes.Slice(4, 4)) == "ftyp",
            _ => false
        };
    }

    private static bool ContainsH264Marker(ReadOnlySpan<byte> bytes)
    {
        var text = Encoding.ASCII.GetString(bytes);
        return text.Contains("avc1", StringComparison.Ordinal) ||
               text.Contains("avc3", StringComparison.Ordinal);
    }

    private static long ReadPositiveLong(string? configured, long fallback)
    {
        return long.TryParse(configured, out var value) && value > 0
            ? value
            : fallback;
    }

    private static BlobServiceClient CreateClient(IConfiguration config)
    {
        var account = config["AzureStorage:AccountName"] ?? "";
        var accessKey = config["AzureStorage:AccessKey"] ?? "";
        var credential = new StorageSharedKeyCredential(account, accessKey);
        return new BlobServiceClient(
            new Uri($"https://{account}.blob.core.windows.net"),
            credential);
    }

    private sealed record MediaFormat(
        string Extension,
        string ContentType,
        IReadOnlyCollection<string> AcceptedContentTypes);
}
