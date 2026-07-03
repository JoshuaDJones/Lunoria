using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Eldoria.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyCharacterItemController(
        IJourneyCharacterItemService journeyCharacterItemService) : ControllerBase
    {
        private readonly IJourneyCharacterItemService _journeyCharacterItemService =
            journeyCharacterItemService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddJourneyCharacterItemRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterItemService.AddJourneyCharacterItem(User.GetUserId(), req.JourneyCharacterId!.Value, req.ItemId!.Value, ct);

            if (result.Success)
                return Ok(result);

            return result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => BadRequest(result.Error),
                "Item.NotFound" => BadRequest(result?.Error),
                "Consumable.CapacityExceeded" => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPatch]
        public async Task<IActionResult> Use([FromBody] UseJourneyCharacterItemRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterItemService.UseJourneyCharacterItem(User.GetUserId(), req.JourneyCharacterItemId!.Value, ct);

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
