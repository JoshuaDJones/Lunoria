using Eldoria.Api.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/scenes/{sceneId:int}/objectives")]
public sealed class SceneObjectiveController(ISceneObjectiveService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(int sceneId, CancellationToken ct)
    {
        var result = await service.ListAsync(User.GetUserId(), sceneId, ct);
        return result.Success ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> Replace(int sceneId, ReplaceSceneObjectivesInput input, CancellationToken ct)
    {
        var result = await service.ReplaceAsync(User.GetUserId(), sceneId, input, ct);
        if (result.Success) return NoContent();
        return result.Error.Code == "Scene.NotFound" ? NotFound(result.Error) : BadRequest(result.Error);
    }
}
