using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyCharacterItemController : ControllerBase
    {
        private readonly IJourneyCharacterItemService _journeyCharacterItemService;

        public JourneyCharacterItemController(IJourneyCharacterItemService journeyCharacterItemService)
        {
            _journeyCharacterItemService = journeyCharacterItemService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddJourneyCharacterItemRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterItemService.AddJourneyCharacterItem(req.JourneyCharacterId!.Value, req.ItemId!.Value, ct);

            if (result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => BadRequest(result.Error),
                "Item.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPatch]
        public async Task<IActionResult> Use([FromBody] UseJourneyCharacterItemRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterItemService.UseJourneyCharacterItem(req.JourneyCharacterItemId!.Value, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "JourneyCharacterItem.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
