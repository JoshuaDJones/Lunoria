using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SceneCharacterController : ControllerBase
    {
        private readonly ISceneCharacterService _sceneCharacterService;

        public SceneCharacterController(ISceneCharacterService sceneCharacterService)
        {
            _sceneCharacterService = sceneCharacterService;
        }

        [HttpPost]
        public async Task<IActionResult> AddSceneCharacter([FromBody] AddSceneCharacterRequest req, CancellationToken ct)
        {
            var result = await _sceneCharacterService.AddSceneCharacterAsync(req.SceneId!.Value, req.CharacterId!.Value, ct);

            if (result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "Scene.NotFound" => BadRequest(result.Error),
                "Character.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{sceneCharacterId:int}")]
        public async Task<IActionResult> Delete(int sceneCharacterId, CancellationToken ct)
        {
            var result = await _sceneCharacterService.DeleteSceneCharacterAsync(sceneCharacterId, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "SceneCharacter.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result?.Error)
            };
        }

        [HttpPatch("{sceneCharacterId:int}")]
        public async Task<IActionResult> Modify(int sceneCharacterId, [FromBody] UpdateSceneCharacterHpMpRequest req, CancellationToken ct)
        {
            var result = await _sceneCharacterService.AdjustCharacterHpMpAsync(sceneCharacterId, req.Hp, req.Mp, ct);

            if(result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "SceneCharacter.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result?.Error)
            };
        }
    }
}
