using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemDto>>> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            CancellationToken ct = default)
        {
            var result = await _itemService.GetListAsync(skip, take, ct);

            if (result.Success)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemDto>> Get(int id, CancellationToken ct)
        {
            var result = await _itemService.GetByIdAsync(id, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Item.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _itemService.DeleteAsync(id, ct);

            if (result.Success)
                return NoContent();

            return result.Error?.Code switch
            {
                "Item.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateItemRequest req, CancellationToken ct)
        {
            var result = await _itemService.CreateAsync(
                req.Name,
                req.Description,
                req.Photo,
                req.HpEffect!.Value,
                req.MpEffect!.Value,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateItemRequest req, CancellationToken ct)
        {
            var result = await _itemService.UpdateAsync(
                id,
                req.Name,
                req.Description,
                req.Photo,
                req.HpEffect!.Value,
                req.MpEffect!.Value,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Item.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}
