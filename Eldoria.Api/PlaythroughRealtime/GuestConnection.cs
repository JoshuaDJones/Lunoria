namespace Eldoria.Api.PlaythroughRealtime;

internal sealed class GuestConnection(string tokenHash, int playthroughId, Action abort)
{
    public string TokenHash { get; } = tokenHash;
    public int PlaythroughId { get; } = playthroughId;
    public Action Abort { get; } = abort;
    public SemaphoreSlim Gate { get; } = new(1, 1);
}
