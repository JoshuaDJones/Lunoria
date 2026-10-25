using Microsoft.AspNetCore.SignalR;

namespace Eldoria.Api.PlaythroughRealtime;

public sealed class PlaythroughRealtimeNotifier(
    GuestConnectionRegistry connections) : IPlaythroughRealtimeNotifier
{
    public Task NotifyUpdatedAsync(
        int playthroughId,
        string reason,
        CancellationToken ct)
    {
        return connections.CheckAsync(playthroughId, reason, ct);
    }

    public Task NotifyClosedAsync(int playthroughId, CancellationToken ct)
    {
       return connections.CheckAsync(playthroughId, null, ct);
    }
}
