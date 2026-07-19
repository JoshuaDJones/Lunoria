using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Eldoria.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SceneCharacterController(ISceneCharacterService sceneCharacterService) : ControllerBase
    {
        private readonly ISceneCharacterService _sceneCharacterService = sceneCharacterService;

        [HttpPost]
        public async Task<IActionResult> AddSceneCharacter([FromBody] AddSceneCharacterRequest req, CancellationToken ct)
        {
            var result = await _sceneCharacterService.AddSceneCharacterAsync(User.GetUserId(), req.SceneId!.Value, req.CharacterId!.Value, ct);

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
            var result = await _sceneCharacterService.DeleteSceneCharacterAsync(User.GetUserId(), sceneCharacterId, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "SceneCharacter.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result?.Error)
            };
        }

    }
}
