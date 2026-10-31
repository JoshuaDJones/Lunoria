using Eldoria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Eldoria.Api.PlaythroughRealtime;

[AllowAnonymous]
public sealed class PlaythroughHub(
    GuestConnectionRegistry connections) : Hub
{
    public Task JoinPlaythrough(string token) => connections.JoinAsync(Context, token);

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        connections.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
