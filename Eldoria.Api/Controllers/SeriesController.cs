using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SeriesController(ISeriesService seriesService) : ControllerBase
    {
        private readonly ISeriesService _seriesService = seriesService;

        [HttpGet]
        public async Task<ActionResult<List<SeriesDto>>> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            CancellationToken ct = default)
        {
            var result = await _seriesService.GetListAsync(User.GetUserId(), skip, take, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SeriesDto>> Get(int id, CancellationToken ct)
        {
            var result = await _seriesService.GetByIdAsync(User.GetUserId(), id, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSeriesRequest req, CancellationToken ct)
        {
            var result = await _seriesService.CreateAsync(User.GetUserId(), req.Name, req.Description, req.Photo, ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, result.Value);

            return ToActionResult(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<SeriesDto>> Update(
            int id,
            [FromForm] UpdateSeriesRequest req,
            CancellationToken ct)
        {
            var result = await _seriesService.UpdateAsync(
                User.GetUserId(), id, req.Name, req.Description, req.Photo, ct);

            return result.Success ? Ok(result.Value) : ToActionResult(result.Error);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _seriesService.DeleteAsync(User.GetUserId(), id, ct);

            return result.Success ? NoContent() : ToActionResult(result.Error);
        }

        private ActionResult ToActionResult(Error error)
        {
            return error.Code switch
            {
                "Series.NotFound" => NotFound(error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(error)
            };
        }
    }
}
