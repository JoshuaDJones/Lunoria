using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SceneController(ISceneService sceneService) : ControllerBase
    {
        private readonly ISceneService _sceneService = sceneService;

        [HttpGet]
        public async Task<ActionResult<List<SceneDto>>> List(
            [FromQuery] int journeyId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 500,
            CancellationToken ct = default)
        {
            var result = await _sceneService.GetListAsync(User.GetUserId(), journeyId, skip, take, ct);

            if (result.Success)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SceneDto>> Get(int id, CancellationToken ct)
        {
            var result = await _sceneService.GetByIdAsync(User.GetUserId(), id, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Scene.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpGet("{id:int}/dashboard")]
        public async Task<ActionResult<SceneDashboardDto>> GetDashboard(int id, [FromQuery] int journeyId, CancellationToken ct)
        {
            var result = await _sceneService.GetSceneDashboardAsync(User.GetUserId(), id, journeyId, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Scene.NotFound" => BadRequest(result.Error),
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int journeyId, CancellationToken ct)
        {
            var result = await _sceneService.DeleteAsync(User.GetUserId(), id, journeyId, ct);

            if (result.Success)
                return NoContent();

            return result.Error?.Code switch
            {
                "Scene.NotFound" => BadRequest(result.Error),
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSceneRequest req, CancellationToken ct)
        {
            var result = await _sceneService.CreateAsync(
                User.GetUserId(),
                req.JourneyId!.Value,
                req.Name,
                req.Description,
                req.Photo,
                req.GridUrl,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, result.Value);

            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateSceneRequest req, CancellationToken ct)
        {
            var result = await _sceneService.UpdateAsync(
                User.GetUserId(),
                req.JourneyId!.Value,
                id,
                req.Name,
                req.Description,
                req.Photo,
                req.GridUrl,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Scene.NotFound" => NotFound(result.Error),
                "Journey.NotFound" => BadRequest(result.Error),
                "Auth.Forbidden" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }
    }
}
