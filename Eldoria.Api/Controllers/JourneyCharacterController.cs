using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Eldoria.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyCharacterController(IJourneyCharacterService journeyCharacterService) : ControllerBase
    {
        private readonly IJourneyCharacterService _journeyCharacterService = journeyCharacterService;

        [HttpPut("{journeyId:int}")]
        public async Task<IActionResult> Replace(int journeyId, [FromBody] ReplaceJourneyCharactersRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterService.ReplaceJourneyCharacters(User.GetUserId(), journeyId, req.CharacterIds, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Character.NotFound" => BadRequest(result.Error),
                "JourneyCharacter.InUse" => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{journeyCharacterId:int}")]
        public async Task<IActionResult> Delete(int journeyCharacterId, CancellationToken ct)
        {
            var result = await _journeyCharacterService.DeleteAsync(User.GetUserId(), journeyCharacterId, ct);

            if (result.Success) 
                return Ok();

            return result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result?.Error)
            };
        }

        [HttpPut("assignment/{journeyCharacterId:int}")]
        public async Task<IActionResult> Update(
            int journeyCharacterId,
            [FromBody] UpdateJourneyCharacterStatsRequest req,
            CancellationToken ct)
        {
            var result = await _journeyCharacterService.UpdateAsync(
                User.GetUserId(), journeyCharacterId,
                req.MeleeAttackDamage, req.BowAttackDamage,
                req.Movement!.Value, req.MaxConsumableInventory!.Value,
                req.MaxEquippableInventory!.Value, req.MaxHp!.Value,
                req.MaxMp!.Value, req.IsInitiallyActive!.Value,
                req.AlternateFormId, ct);

            return result.Success ? Ok(result.Value) : result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
