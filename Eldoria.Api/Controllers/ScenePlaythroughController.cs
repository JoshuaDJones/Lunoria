using Eldoria.Api.Common;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers;

[Route("api/v1/playthroughs/{playthroughId:int}/scenes/{sceneId:int}")]
[ApiController]
public sealed class ScenePlaythroughController(
    IScenePlaythroughService scenePlaythroughService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ScenePlaythroughDetailsDto>> Get(
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.GetAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            ct);

        if (result.Success)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "ScenePlaythrough.NotFound" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(
        int playthroughId,
        int sceneId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.StartAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            ct);

        return result.Success ? NoContent() : ToError(result.Error);
    }

    [HttpPost("participants/scene-characters/{scenePlaythroughCharacterId:int}")]
    public async Task<IActionResult> AddSceneCharacterInstance(
        int playthroughId,
        int sceneId,
        int scenePlaythroughCharacterId,
        CancellationToken ct)
    {
        var result = await scenePlaythroughService.AddSceneCharacterInstanceAsync(
            User.GetUserId(),
            playthroughId,
            sceneId,
            scenePlaythroughCharacterId,
            ct);

        return result.Success ? NoContent() : ToError(result.Error);
    }

    private IActionResult ToError(Error error) => error.Code switch
    {
        "ScenePlaythrough.NotFound" => NotFound(error),
        "ScenePlaythrough.AlreadyStarted" => Conflict(error),
        "ScenePlaythrough.InvalidState" => Conflict(error),
        "ScenePlaythrough.NotInProgress" => Conflict(error),
        "ScenePlaythrough.CharacterNotFound" => NotFound(error),
        "ScenePlaythrough.InvalidCharacterType" => BadRequest(error),
        "Playthrough.Completed" => Conflict(error),
        _ => BadRequest(error)
    };
}
