using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Eldoria.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SceneCharacterItemController(
        ISceneCharacterItemService sceneCharacterItemService) : ControllerBase
    {
        private readonly ISceneCharacterItemService _sceneCharacterItemService =
            sceneCharacterItemService;

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddSceneCharacterItemRequest req, CancellationToken ct)
        {
            var result = await _sceneCharacterItemService.AddItem(User.GetUserId(), req.SceneCharacterId!.Value, req.ItemId!.Value, ct);

            if(result.Success) 
                return Ok();

            return result.Error?.Code switch
            {
                "SceneCharacter.NotFound" => NotFound(result.Error),
                "Item.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPatch]
        public async Task<IActionResult> UseItem([FromBody] UseSceneCharacterItemRequest req, CancellationToken ct)
        {
            var result = await _sceneCharacterItemService.UseItem(User.GetUserId(), req.SceneCharacterItemId!.Value, ct);

            if(result.Success) 
                return Ok();

            return result.Error?.Code switch
            {
                "SceneCharacterItem.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
