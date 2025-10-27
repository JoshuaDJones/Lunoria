using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SpellController : ControllerBase
    {
        private readonly ISpellService _spellService;

        public SpellController(ISpellService spellService)
        {
            _spellService = spellService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SpellDto>>> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            CancellationToken ct = default)
        {
            var result = await _spellService.GetListAsync(skip, take, ct);

            if (result.Success)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SpellDto>> Get(int id, CancellationToken ct)
        {
            var result = await _spellService.GetByIdAsync(id, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Spell.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _spellService.DeleteAsync(id, ct);

            if (result.Success)
                return NoContent();

            return result.Error?.Code switch
            {
                "Spell.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSpellRequest req, CancellationToken ct)
        {
            var result = await _spellService.CreateAsync(
                req.Name,
                req.Description,
                req.Photo,
                req.Range!.Value,
                req.IsRadius!.Value,
                req.MpCost!.Value,
                req.DamageEffect,
                req.HealthEffect,
                req.MagicEffect,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateSpellRequest req, CancellationToken ct)
        {
            var result = await _spellService.UpdateAsync(
                id,
                req.Name,
                req.Description,
                req.Photo,
                req.Range!.Value,
                req.IsRadius!.Value,
                req.MpCost!.Value,
                req.DamageEffect,
                req.HealthEffect,
                req.MagicEffect,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Spell.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
