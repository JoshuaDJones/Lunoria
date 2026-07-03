using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EquippableItemController(IEquippableItemService equippableItemService) : ControllerBase
    {
        private readonly IEquippableItemService _equippableItemService = equippableItemService;

        [HttpGet]
        public async Task<ActionResult<List<EquippableItemDto>>> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            CancellationToken ct = default)
        {
            var result = await _equippableItemService.GetListAsync(
                User.GetUserId(),
                skip,
                take,
                ct);

            return result.Success ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EquippableItemDto>> Get(int id, CancellationToken ct)
        {
            var result = await _equippableItemService.GetByIdAsync(User.GetUserId(), id, ct);

            return result.Success ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<EquippableItemDto>> Create(
            [FromForm] CreateEquippableItemRequest request,
            CancellationToken ct)
        {
            var result = await _equippableItemService.CreateAsync(
                User.GetUserId(),
                request.ToInput(request.Photo),
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<EquippableItemDto>> Update(
            int id,
            [FromForm] UpdateEquippableItemRequest request,
            CancellationToken ct)
        {
            var result = await _equippableItemService.UpdateAsync(
                User.GetUserId(),
                id,
                request.ToInput(request.Photo),
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error.Code == "EquippableItem.NotFound"
                ? NotFound(result.Error)
                : BadRequest(result.Error);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _equippableItemService.DeleteAsync(User.GetUserId(), id, ct);

            if (result.Success)
                return NoContent();

            return result.Error.Code switch
            {
                "EquippableItem.NotFound" => NotFound(result.Error),
                "EquippableItem.InUse" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }
    }
}
