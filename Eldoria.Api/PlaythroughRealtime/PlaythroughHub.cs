using Eldoria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Eldoria.Api.PlaythroughRealtime;

[AllowAnonymous]
public sealed class PlaythroughHub(
    IPlaythroughJoinSessionService joinSessionService) : Hub
{
    private const string JoinedGroupItemKey = "playthrough-group";

    public async Task JoinPlaythrough(string token)
    {
        var result = await joinSessionService.ValidateAsync(
            token,
            Context.ConnectionAborted);
        if (!result.Success)
            throw new HubException(result.Error.Message);

        if (Context.Items.TryGetValue(JoinedGroupItemKey, out var previous) &&
            previous is string previousGroup)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, previousGroup);
        }

        var group = GroupName(result.Value);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        Context.Items[JoinedGroupItemKey] = group;
    }

    public static string GroupName(int playthroughId) =>
        $"playthrough:{playthroughId}";
}
