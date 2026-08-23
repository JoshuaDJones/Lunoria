using System.Security.Cryptography;
using System.Text;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services;

public sealed class PlaythroughJoinSessionService(
    IPlaythroughJoinSessionRepository repository)
    : IPlaythroughJoinSessionService
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(24);

    public async Task<Result<PlaythroughJoinSessionDto>> CreateAsync(
        int userId,
        int playthroughId,
        CancellationToken ct)
    {
        var existingSession = await repository.GetForOwnerAsync(
            userId,
            playthroughId,
            ct);
        var playthrough = existingSession?.Playthrough ??
            await repository.GetOwnedPlaythroughAsync(userId, playthroughId, ct);

        if (playthrough is null)
        {
            return Result<PlaythroughJoinSessionDto>.Fail(new Error(
                "Playthrough.NotFound",
                "Playthrough was not found."));
        }

        if (playthrough.CompletedAt is not null)
        {
            return Result<PlaythroughJoinSessionDto>.Fail(new Error(
                "Playthrough.Completed",
                "A completed playthrough cannot accept new guests."));
        }

        var token = GenerateToken();
        var now = DateTime.UtcNow;
        var expiresAt = now.Add(SessionLifetime);

        if (existingSession is null)
        {
            await repository.AddAsync(new PlaythroughJoinSession
            {
                TokenHash = HashToken(token),
                CreatedAt = now,
                ExpiresAt = expiresAt,
                PlaythroughId = playthroughId
            }, ct);
        }
        else
        {
            existingSession.TokenHash = HashToken(token);
            existingSession.CreatedAt = now;
            existingSession.ExpiresAt = expiresAt;
            existingSession.RevokedAt = null;
        }

        await repository.SaveChangesAsync(ct);
        return Result<PlaythroughJoinSessionDto>.Ok(new PlaythroughJoinSessionDto
        {
            Token = token,
            ExpiresAt = expiresAt
        });
    }

    public async Task<Result> RevokeAsync(
        int userId,
        int playthroughId,
        CancellationToken ct)
    {
        var session = await repository.GetForOwnerAsync(
            userId,
            playthroughId,
            ct);
        if (session is null)
        {
            var playthrough = await repository.GetOwnedPlaythroughAsync(
                userId,
                playthroughId,
                ct);
            return playthrough is null
                ? Result.Fail(new Error(
                    "Playthrough.NotFound",
                    "Playthrough was not found."))
                : Result.Ok();
        }

        session.RevokedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(ct);
        return Result.Ok();
    }

    public async Task<Result<PublicPlaythroughSnapshotDto>> GetPublicSnapshotAsync(
        string token,
        CancellationToken ct)
    {
        if (!IsPlausibleToken(token))
            return InvalidSnapshotToken();

        var playthrough = await repository.GetPublicSnapshotAsync(
            HashToken(token),
            DateTime.UtcNow,
            ct);

        return playthrough is null
            ? InvalidSnapshotToken()
            : Result<PublicPlaythroughSnapshotDto>.Ok(
                playthrough.ToPublicSnapshotDto());
    }

    public async Task<Result<int>> ValidateAsync(
        string token,
        CancellationToken ct)
    {
        if (!IsPlausibleToken(token))
            return InvalidToken();

        var session = await repository.GetValidByTokenHashAsync(
            HashToken(token),
            DateTime.UtcNow,
            ct);

        return session is null
            ? InvalidToken()
            : Result<int>.Ok(session.PlaythroughId);
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    private static bool IsPlausibleToken(string token)
    {
        return !string.IsNullOrWhiteSpace(token) && token.Length is >= 32 and <= 128;
    }

    private static Result<int> InvalidToken()
    {
        return Result<int>.Fail(new Error(
            "PlaythroughJoin.InvalidOrExpired",
            "This playthrough invitation is invalid or has expired."));
    }

    private static Result<PublicPlaythroughSnapshotDto> InvalidSnapshotToken()
    {
        return Result<PublicPlaythroughSnapshotDto>.Fail(new Error(
            "PlaythroughJoin.InvalidOrExpired",
            "This playthrough invitation is invalid or has expired."));
    }
}
