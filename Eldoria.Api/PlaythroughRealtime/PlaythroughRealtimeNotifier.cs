using Microsoft.AspNetCore.SignalR;

namespace Eldoria.Api.PlaythroughRealtime;

public sealed class PlaythroughRealtimeNotifier(
    IHubContext<PlaythroughHub> hubContext) : IPlaythroughRealtimeNotifier
{
    public Task NotifyUpdatedAsync(
        int playthroughId,
        string reason,
        CancellationToken ct)
    {
        return hubContext.Clients
            .Group(PlaythroughHub.GroupName(playthroughId))
            .SendAsync("PlaythroughUpdated", new { reason }, ct);
    }

    public Task NotifyClosedAsync(int playthroughId, CancellationToken ct)
    {
        return hubContext.Clients
            .Group(PlaythroughHub.GroupName(playthroughId))
            .SendAsync("SessionClosed", ct);
    }
}
