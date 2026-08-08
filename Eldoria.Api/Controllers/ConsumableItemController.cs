using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConsumableItemController(IConsumableItemService consumableItemService) : ControllerBase
    {
        private readonly IConsumableItemService _consumableItemService = consumableItemService;

        [HttpGet]
        public async Task<ActionResult<List<ConsumableItemDto>>> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            CancellationToken ct = default)
        {
            var result = await _consumableItemService.GetListAsync(
                User.GetUserId(), skip, take, ct);

            return result.Success ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConsumableItemDto>> Get(
            int id,
            CancellationToken ct)
        {
            var result = await _consumableItemService.GetByIdAsync(
                User.GetUserId(), id, ct);

            return result.Success ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ConsumableItemDto>> Create(
            [FromForm] CreateConsumableItemRequest request,
            CancellationToken ct)
        {
            var result = await _consumableItemService.CreateAsync(
                User.GetUserId(),
                request.Name,
                request.Description,
                request.Photo,
                request.HpEffect!.Value,
                request.MpEffect!.Value,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ConsumableItemDto>> Update(
            int id,
            [FromForm] UpdateConsumableItemRequest request,
            CancellationToken ct)
        {
            var result = await _consumableItemService.UpdateAsync(
                User.GetUserId(),
                id,
                request.Name,
                request.Description,
                request.Photo,
                request.HpEffect!.Value,
                request.MpEffect!.Value,
                ct);

            return result.Success ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _consumableItemService.DeleteAsync(
                User.GetUserId(), id, ct);

            if (result.Success)
                return NoContent();

            return result.Error.Code switch
            {
                "ConsumableItem.NotFound" => NotFound(result.Error),
                "ConsumableItem.InUse" => Conflict(result.Error),
                _ => BadRequest(result.Error),
            };
        }
    }
}
