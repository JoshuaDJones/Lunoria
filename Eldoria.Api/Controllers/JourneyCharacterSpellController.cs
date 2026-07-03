using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/journey-characters/{journeyCharacterId:int}/spells")]
    [ApiController]
    public class JourneyCharacterSpellController(
        IJourneyCharacterSpellService journeyCharacterSpellService) : ControllerBase
    {
        private readonly IJourneyCharacterSpellService _journeyCharacterSpellService =
            journeyCharacterSpellService;

        [HttpPost]
        public async Task<ActionResult<JourneyCharacterSpellDto>> Grant(
            int journeyCharacterId,
            [FromBody] GrantJourneyCharacterSpellRequest request,
            CancellationToken ct)
        {
            var result = await _journeyCharacterSpellService.GrantAsync(
                User.GetUserId(),
                journeyCharacterId,
                request.SpellId!.Value,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code switch
            {
                "JourneyCharacter.NotFound" => NotFound(result.Error),
                "Spell.NotFound" => NotFound(result.Error),
                "JourneyCharacterSpell.AlreadyGranted" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }

        [HttpDelete("{spellId:int}")]
        public async Task<IActionResult> Remove(
            int journeyCharacterId,
            int spellId,
            CancellationToken ct)
        {
            var result = await _journeyCharacterSpellService.RemoveAsync(
                User.GetUserId(),
                journeyCharacterId,
                spellId,
                ct);

            if (result.Success)
                return NoContent();

            return result.Error.Code switch
            {
                "JourneyCharacterSpell.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error),
            };
        }
    }
}
