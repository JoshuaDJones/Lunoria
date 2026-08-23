using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface IPlaythroughJoinSessionService
{
    Task<Result<PlaythroughJoinSessionDto>> CreateAsync(
        int userId,
        int playthroughId,
        CancellationToken ct);

    Task<Result> RevokeAsync(
        int userId,
        int playthroughId,
        CancellationToken ct);

    Task<Result<PublicPlaythroughSnapshotDto>> GetPublicSnapshotAsync(
        string token,
        CancellationToken ct);

    Task<Result<int>> ValidateAsync(string token, CancellationToken ct);
}
