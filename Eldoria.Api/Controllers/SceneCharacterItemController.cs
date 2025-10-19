using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SceneCharacterItemController : ControllerBase
    {
        private readonly ISceneCharacterItemService _sceneCharacterItemService;

        public SceneCharacterItemController(ISceneCharacterItemService sceneCharacterItemService)
        {   
            _sceneCharacterItemService = sceneCharacterItemService;
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddSceneCharacterItemRequest req, CancellationToken ct)
        {
            var result = await _sceneCharacterItemService.AddItem(req.SceneCharacterId, req.ItemId, ct);

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
            var result = await _sceneCharacterItemService.UseItem(req.SceneCharacterItemId, ct); 

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
