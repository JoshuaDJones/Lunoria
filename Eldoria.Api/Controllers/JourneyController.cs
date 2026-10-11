using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyController(IJourneyService journeyService, IJourneyCharacterSyncSummaryService characterSyncSummaryService) : ControllerBase
    {
        private readonly IJourneyService _journeyService = journeyService;

        [Authorize]
        [HttpGet("{id:int}/character-sync-summary")]
        public async Task<ActionResult<JourneyCharacterSyncSummaryDto>> CharacterSyncSummary(int id, CancellationToken ct)
        {
            var result = await characterSyncSummaryService.GetAsync(User.GetUserId(), id, ct);
            return result.Success ? Ok(result.Value) : result.Error.Code switch
            {
                "Journey.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpGet]
        public async Task<ActionResult<List<JourneyDto>>> List([FromQuery] int skip = 0, [FromQuery] int take = 500, CancellationToken ct = default)
        {
            var result = await _journeyService.GetListAsync(User.GetUserId(), skip, take, ct);

            if(result.Success)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<JourneyDto>> Get(int id, CancellationToken ct)
        {
            var result = await _journeyService.GetByIdAsync(User.GetUserId(), id, ct);

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
            var result = await _journeyService.DeleteAsync(User.GetUserId(), id, ct);

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
            var result = await _journeyService.CreateAsync(User.GetUserId(), req.SeriesId, req.Name, req.Description, req.Photo, ct);

            if(result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateJourneyRequest req, CancellationToken ct)
        {
            var result = await _journeyService.UpdateAsync(id, User.GetUserId(), req.Name, req.Description, req.Photo, ct);

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
