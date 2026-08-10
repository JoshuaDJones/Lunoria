using Microsoft.AspNetCore.SignalR;

namespace Eldoria.Api.GridPrototype;

public sealed class GridPrototypeHub(GridPrototypeSessionStore sessions) : Hub
{
    public async Task<CreateGridPrototypeSessionResult> CreateSession()
    {
        await LeavePreviousGroup();
        var result = sessions.Create(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(result.Session.Code));
        return result;
    }

    public async Task<CreateGridPrototypeSessionResult> CreateConfiguredSession(
        int rows,
        int columns,
        string gridColor,
        string? backgroundImage)
    {
        await LeavePreviousGroup();
        var result = await Invoke(() => sessions.Create(
            Context.ConnectionId,
            rows,
            columns,
            gridColor,
            backgroundImage));
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(result.Session.Code));
        return result;
    }

    public async Task<GridPrototypeSessionSnapshot> JoinSession(string code)
    {
        await LeavePreviousGroup();
        var snapshot = await Invoke(() => sessions.Join(Context.ConnectionId, code));
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(snapshot.Code));
        return snapshot;
    }

    public Task<bool> BeginMove(string code, string tokenId) =>
        Invoke(() => sessions.BeginMove(Context.ConnectionId, code, tokenId));

    public async Task MoveToken(string code, string tokenId, int row, int column)
    {
        var snapshot = await Invoke(() => sessions.Move(Context.ConnectionId, code, tokenId, row, column));
        await Broadcast(snapshot);
    }

    public Task EndMove(string code, string tokenId) =>
        Invoke(() => sessions.EndMove(Context.ConnectionId, code, tokenId));

    public async Task AddToken(string code, string hostToken, GridPrototypeCharacterDto character)
    {
        var snapshot = await Invoke(() => sessions.AddToken(code, hostToken, character));
        await Broadcast(snapshot);
    }

    public async Task RemoveToken(string code, string hostToken, string tokenId)
    {
        var snapshot = await Invoke(() => sessions.RemoveToken(code, hostToken, tokenId));
        await Broadcast(snapshot);
    }

    public async Task SetBackground(string code, string hostToken, string? backgroundImage)
    {
        var snapshot = await Invoke(() => sessions.SetBackground(code, hostToken, backgroundImage));
        await Broadcast(snapshot);
    }

    public async Task SetGridColor(string code, string hostToken, string gridColor)
    {
        var snapshot = await Invoke(() => sessions.SetGridColor(code, hostToken, gridColor));
        await Broadcast(snapshot);
    }

    public async Task CloseSession(string code, string hostToken)
    {
        var closedCode = await Invoke(() => sessions.Close(code, hostToken));
        await Clients.Group(GroupName(closedCode)).SendAsync("SessionClosed");
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        sessions.Disconnect(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    private async Task LeavePreviousGroup()
    {
        var previousCode = sessions.GetJoinedCode(Context.ConnectionId);
        if (previousCode is not null)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(previousCode));
    }

    private Task Broadcast(GridPrototypeSessionSnapshot snapshot) =>
        Clients.Group(GroupName(snapshot.Code)).SendAsync("BoardUpdated", snapshot);

    private static string GroupName(string code) => $"grid-prototype:{code.ToUpperInvariant()}";

    private static Task Invoke(Action action)
    {
        try
        {
            action();
            return Task.CompletedTask;
        }
        catch (GridPrototypeException exception)
        {
            throw new HubException(exception.Message);
        }
    }

    private static Task<T> Invoke<T>(Func<T> action)
    {
        try
        {
            return Task.FromResult(action());
        }
        catch (GridPrototypeException exception)
        {
            throw new HubException(exception.Message);
        }
    }
}
