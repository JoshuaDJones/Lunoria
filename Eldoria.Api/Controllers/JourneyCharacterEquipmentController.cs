using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyCharacterEquipmentController(
        IJourneyCharacterEquipmentService equipmentService) : ControllerBase
    {
        private readonly IJourneyCharacterEquipmentService _equipmentService =
            equipmentService;

        [HttpPost]
        public async Task<ActionResult<JourneyCharacterEquippableItemDto>> Add(
            [FromBody] AddJourneyCharacterEquipmentRequest request,
            CancellationToken ct)
        {
            var result = await _equipmentService.AddAsync(
                User.GetUserId(),
                request.JourneyCharacterId!.Value,
                request.EquippableItemId!.Value,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code switch
            {
                "JourneyCharacter.NotFound" => NotFound(result.Error),
                "EquippableItem.NotFound" => NotFound(result.Error),
                "Equipment.CapacityExceeded" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }

        [HttpPatch("{assignmentId:int}")]
        public async Task<ActionResult<JourneyCharacterEquippableItemDto>> SetEquipped(
            int assignmentId,
            [FromBody] SetJourneyCharacterEquipmentStateRequest request,
            CancellationToken ct)
        {
            var result = await _equipmentService.SetEquippedAsync(
                User.GetUserId(),
                assignmentId,
                request.IsEquipped!.Value,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code switch
            {
                "JourneyCharacterEquipment.NotFound" => NotFound(result.Error),
                "Equipment.UnsafeCapacityReduction" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }

        [HttpDelete("{assignmentId:int}")]
        public async Task<IActionResult> Remove(int assignmentId, CancellationToken ct)
        {
            var result = await _equipmentService.RemoveAsync(
                User.GetUserId(),
                assignmentId,
                ct);

            if (result.Success)
                return NoContent();

            return result.Error.Code switch
            {
                "JourneyCharacterEquipment.NotFound" => NotFound(result.Error),
                "Equipment.UnsafeCapacityReduction" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }
    }
}
