using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyCharacterController : ControllerBase
    {
        private readonly IJourneyCharacterService _journeyCharacterService;

        public JourneyCharacterController(IJourneyCharacterService journeyCharacterService)
        {
            _journeyCharacterService = journeyCharacterService;
        }

        [HttpPut("{journeyId:int}")]
        public async Task<IActionResult> Replace(int journeyId, [FromBody] ReplaceJourneyCharactersRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterService.ReplaceJourneyCharacters(journeyId, req.CharacterIds, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Character.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPatch("{journeyCharacterId:int}")]
        public async Task<IActionResult> Modify(int journeyCharacterId, [FromBody] UpdateJourneyCharacterHpMpRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterService.AdjustCharacterHpMpAsync(journeyCharacterId, req.Hp!.Value, req.Mp!.Value, ct);

            if (result.Success) 
                return Ok();

            return result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result?.Error)
            };
        }

        [HttpDelete("{journeyCharacterId:int}")]
        public async Task<IActionResult> Delete(int journeyCharacterId, CancellationToken ct)
        {
            var result = await _journeyCharacterService.DeleteAsync(journeyCharacterId, ct);

            if (result.Success) 
                return Ok();

            return result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result?.Error)
            };
        }
    }
}
