namespace Eldoria.Api.PlaythroughRealtime;

public interface IPlaythroughRealtimeNotifier
{
    Task NotifyUpdatedAsync(
        int playthroughId,
        string reason,
        CancellationToken ct);

    Task NotifyClosedAsync(int playthroughId, CancellationToken ct);
}
