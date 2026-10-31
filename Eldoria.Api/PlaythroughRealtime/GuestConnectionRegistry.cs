using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Eldoria.Api.PlaythroughRealtime;

public sealed class GuestConnectionRegistry(
    IServiceScopeFactory scopeFactory,
    IHubContext<PlaythroughHub> hubContext)
{
    private readonly ConcurrentDictionary<string, GuestConnection> connections = new();

    public async Task JoinAsync(HubCallerContext context, string token)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length is < 32 or > 128)
            throw new HubException("This playthrough invitation is invalid or has expired.");

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        using var scope = scopeFactory.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IPlaythroughJoinSessionRepository>();

        var session = await repository.GetValidByTokenHashAsync(hash, DateTime.UtcNow, context.ConnectionAborted);

        if (session is null)
            throw new HubException("This playthrough invitation is invalid or has expired.");

        connections.TryGetValue(context.ConnectionId, out var previous);

        if (previous is not null)
            await previous.Gate.WaitAsync(context.ConnectionAborted);

        try
        {
            context.ConnectionAborted.ThrowIfCancellationRequested();

            connections[context.ConnectionId] = new GuestConnection(hash, session.PlaythroughId, context.Abort);

            if (context.ConnectionAborted.IsCancellationRequested)
                Remove(context.ConnectionId);
        }
        finally 
        { 
            previous?.Gate.Release(); 
        }
    }

    public void Remove(string connectionId) 
        => connections.TryRemove(connectionId, out _);

    public async Task CheckAsync(int? playthroughId, string? reason, CancellationToken ct)
    {
        foreach (var (id, connection) in connections.ToArray())
        {
            if (playthroughId.HasValue && connection.PlaythroughId != playthroughId.Value)
                continue;

            await connection.Gate.WaitAsync(ct);

            try
            {
                if (!connections.TryGetValue(id, out var current) || !ReferenceEquals(current, connection))
                    continue;

                using var scope = scopeFactory.CreateScope();

                var repository = scope.ServiceProvider.GetRequiredService<IPlaythroughJoinSessionRepository>();

                var session = await repository.GetValidByTokenHashAsync(
                    connection.TokenHash, DateTime.UtcNow, ct);

                if (session is null || session.PlaythroughId != connection.PlaythroughId ||
                    session.ExpiresAt <= DateTime.UtcNow)
                {
                    // Abort regardless of whether the client handles SessionClosed.
                    try 
                    { 
                        await hubContext.Clients.Client(id).SendAsync("SessionClosed", ct); 
                    }
                    finally
                    {
                        ((ICollection<KeyValuePair<string, GuestConnection>>)connections)
                            .Remove(new(id, connection));

                        connection.Abort();
                    }

                    continue;
                }

                if (reason is not null)
                    await hubContext.Clients.Client(id).SendAsync("PlaythroughUpdated", new { reason }, ct);
            }
            finally 
            { 
                connection.Gate.Release(); 
            }
        }
    }
}
