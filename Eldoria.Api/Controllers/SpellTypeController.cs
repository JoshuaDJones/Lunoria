using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SpellTypeController(ISpellTypeService spellTypeService) : ControllerBase
    {
        private readonly ISpellTypeService _spellTypeService = spellTypeService;

        [HttpGet]
        public async Task<ActionResult<List<SpellTypeDto>>> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            CancellationToken ct = default)
        {
            var result = await _spellTypeService.GetListAsync(
                User.GetUserId(),
                skip,
                take,
                ct);

            return result.Success ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SpellTypeDto>> Get(int id, CancellationToken ct)
        {
            var result = await _spellTypeService.GetByIdAsync(User.GetUserId(), id, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code == "SpellType.NotFound"
                ? NotFound(result.Error)
                : BadRequest(result.Error);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<SpellTypeDto>> Create(
            [FromForm] CreateSpellTypeRequest request,
            CancellationToken ct)
        {
            var result = await _spellTypeService.CreateAsync(
                User.GetUserId(),
                request.Name,
                request.Description,
                request.Photo,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);

            return result.Error.Code == "SpellType.NameExists"
                ? Conflict(result.Error)
                : BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<SpellTypeDto>> Update(
            int id,
            [FromForm] UpdateSpellTypeRequest request,
            CancellationToken ct)
        {
            var result = await _spellTypeService.UpdateAsync(
                User.GetUserId(),
                id,
                request.Name,
                request.Description,
                request.Photo,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code switch
            {
                "SpellType.NotFound" => NotFound(result.Error),
                "SpellType.NameExists" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _spellTypeService.DeleteAsync(User.GetUserId(), id, ct);

            if (result.Success)
                return NoContent();

            return result.Error.Code switch
            {
                "SpellType.NotFound" => NotFound(result.Error),
                "SpellType.InUse" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }
    }
}
