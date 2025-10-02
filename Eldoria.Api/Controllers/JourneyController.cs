using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyController : ControllerBase
    {
        private readonly IJourneyService _journeyService;
        public JourneyController(IJourneyService journeyService)
        {
            _journeyService = journeyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<JourneyDto>>> List([FromQuery] int skip = 0, [FromQuery] int take = 500, CancellationToken ct = default)
        {
            var userId = User.GetUserId();
            var result = await _journeyService.GetListAsync(userId, skip, take, ct);

            if(result.Success)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<JourneyDto>> Get(int id, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _journeyService.GetByIdAsync(userId, id, ct);

            if(result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _journeyService.DeleteAsync(userId, id, ct);

            if (result.Success)
                return NoContent();

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateJourneyRequest req, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _journeyService.CreateAsync(userId, req.Name, req.Description, req.Photo, ct);

            if(result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateJourneyRequest req, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _journeyService.UpdateAsync(id, userId, req.Name, req.Description, req.Photo, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Journey.NotFound" => NotFound(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }
    }
}
