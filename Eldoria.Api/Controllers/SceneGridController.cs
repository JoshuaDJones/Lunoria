using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/scenes/{sceneId:int}/grid")]
    [ApiController]
    public class SceneGridController(ISceneGridService sceneGridService) : ControllerBase
    {
        private readonly ISceneGridService _sceneGridService = sceneGridService;

        [HttpGet]
        public async Task<IActionResult> Get(int sceneId, CancellationToken ct)
        {
            var result = await _sceneGridService.GetAsync(User.GetUserId(), sceneId, ct);
            return result.Success ? Ok(result.Value) : ToError(result.Error);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
            int sceneId,
            [FromForm] CreateSceneGridRequest request,
            CancellationToken ct)
        {
            var result = await _sceneGridService.CreateAsync(
                User.GetUserId(),
                sceneId,
                request.Rows!.Value,
                request.Columns!.Value,
                request.GridColor,
                request.Background,
                ct);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { sceneId }, result.Value)
                : ToError(result.Error);
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int sceneId,
            [FromForm] UpdateSceneGridRequest request,
            CancellationToken ct)
        {
            var result = await _sceneGridService.UpdateAsync(
                User.GetUserId(),
                sceneId,
                request.Rows!.Value,
                request.Columns!.Value,
                request.GridColor,
                request.Background,
                request.RemoveBackground,
                ct);

            return result.Success ? Ok(result.Value) : ToError(result.Error);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int sceneId, CancellationToken ct)
        {
            var result = await _sceneGridService.DeleteAsync(User.GetUserId(), sceneId, ct);
            return result.Success ? NoContent() : ToError(result.Error);
        }

        private IActionResult ToError(Error? error) => error?.Code switch
        {
            "Scene.NotFound" => NotFound(error),
            "SceneGrid.NotFound" => NotFound(error),
            "SceneGrid.AlreadyExists" => Conflict(error),
            _ => BadRequest(error),
        };
    }
}
