using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Eldoria.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CharacterDto>>> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            [FromQuery] CharacterType typeFilter = CharacterType.Any,
            CancellationToken ct = default)
        {
            var result = await _characterService.GetListAsync(skip, take, typeFilter, ct);

            if (result.Success)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CharacterDto>> Get(int id, CancellationToken ct)
        {
            var result = await _characterService.GetByIdAsync(id, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Character.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _characterService.DeleteAsync(id, ct);

            if (result.Success)
                return NoContent();

            return result.Error?.Code switch
            {
                "Character.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCharacterRequest req, CancellationToken ct)
        {
            var result = await _characterService.CreateAsync(
                req.Name,
                req.Description,
                req.Photo,
                req.MaxHp!.Value,
                req.MaxMp!.Value,
                req.MeleeAttackDamage,
                req.BowAttackDamage,
                req.Movement!.Value,
                req.MaxInventory!.Value,
                req.IsPlayer!.Value,
                req.IsNPC!.Value,
                req.IsEnemy!.Value,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateCharacterRequest req, CancellationToken ct)
        {
            var result = await _characterService.UpdateAsync(
                id,
                req.Name,
                req.Description,
                req.Photo,
                req.MaxHp!.Value,
                req.MaxMp!.Value,
                req.MeleeAttackDamage,
                req.BowAttackDamage,
                req.Movement!.Value,
                req.MaxInventory!.Value,
                req.IsPlayer!.Value,
                req.IsNPC!.Value,
                req.IsEnemy!.Value,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Character.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
