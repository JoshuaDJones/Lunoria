using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CharacterSpellsController : ControllerBase
    {
        private readonly ICharacterSpellService _characterSpellService;

        public CharacterSpellsController(ICharacterSpellService characterSpellService)
        {
            _characterSpellService = characterSpellService;
        }

        [HttpPut("{characterId:int}")]
        public async Task<IActionResult> Replace(int characterId, [FromBody] ReplaceCharacterSpellsRequest req, CancellationToken ct)
        {
            var result = await _characterSpellService.ReplaceCharacterSpells(characterId, req.SpellIds, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "Character.NotFound" => BadRequest(result.Error),
                "Spell.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
