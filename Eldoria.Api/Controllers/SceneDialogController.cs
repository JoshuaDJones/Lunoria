using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SceneDialogController : ControllerBase
    {
        private readonly ISceneDialogService _sceneDialogService;

        public SceneDialogController(ISceneDialogService sceneDialogService)
        {
            _sceneDialogService = sceneDialogService;
        }

        [HttpPost("{sceneId:int}")]
        public async Task<IActionResult> Post(int sceneId, [FromBody] CreateSceneDialogRequest req, CancellationToken ct)
        {
            var result = await _sceneDialogService.CreateSceneDialogAsync(sceneId, req.Title, ct);

            if(result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "Scene.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpGet("{sceneId:int}")]
        public async Task<ActionResult<List<SceneDialogDto>>> Get(int sceneId, CancellationToken ct)
        {
            var result = await _sceneDialogService.GetSceneDialogsAsync(sceneId, ct);

            if (result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "Scene.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{sceneDialogId:int}")]
        public async Task<IActionResult> Delete(int sceneDialogId, CancellationToken ct)
        {
            var result = await _sceneDialogService.DeleteSceneDialogAsync(sceneDialogId, ct);

            if (result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "SceneDialog.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPatch("{sceneDialogId:int}")]
        public async Task<IActionResult> Edit(int sceneDialogId, [FromBody] UpdateSceneDialogRequest req, CancellationToken ct)
        {
            var result = await _sceneDialogService.EditSceneDialogAsync(sceneDialogId, req.Title, ct);

            if (result.Success) 
                return Ok(result);

            return result.Error?.Code switch
            {
                "SceneDialog.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
