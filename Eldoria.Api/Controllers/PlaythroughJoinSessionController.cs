using Eldoria.Api.Common;
using Eldoria.Api.PlaythroughRealtime;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Eldoria.Api.Controllers;

[ApiController]
public sealed class PlaythroughJoinSessionController(
    IPlaythroughJoinSessionService joinSessionService,
    IPlaythroughRealtimeNotifier realtimeNotifier) : ControllerBase
{
    [HttpPost("api/v1/playthroughs/{playthroughId:int}/join-session")]
    public async Task<ActionResult<PlaythroughJoinSessionDto>> Create(
        int playthroughId,
        CancellationToken ct)
    {
        var result = await joinSessionService.CreateAsync(
            User.GetUserId(),
            playthroughId,
            ct);

        if (result.Success)
        {
            await realtimeNotifier.NotifyClosedAsync(playthroughId, ct);
            return Ok(result.Value);
        }

        return result.Error.Code switch
        {
            "Playthrough.NotFound" => NotFound(result.Error),
            "Playthrough.Completed" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpDelete("api/v1/playthroughs/{playthroughId:int}/join-session")]
    public async Task<IActionResult> Revoke(
        int playthroughId,
        CancellationToken ct)
    {
        var result = await joinSessionService.RevokeAsync(
            User.GetUserId(),
            playthroughId,
            ct);
        if (!result.Success)
            return result.Error.Code == "Playthrough.NotFound"
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        await realtimeNotifier.NotifyClosedAsync(playthroughId, ct);
        return NoContent();
    }

    [AllowAnonymous]
    [EnableRateLimiting("public-playthrough")]
    [HttpGet("api/v1/playthrough-sessions/{token}")]
    public async Task<ActionResult<PublicPlaythroughSnapshotDto>> GetPublic(
        string token,
        CancellationToken ct)
    {
        var result = await joinSessionService.GetPublicSnapshotAsync(token, ct);
        return result.Success
            ? Ok(result.Value)
            : NotFound(result.Error);
    }
}
