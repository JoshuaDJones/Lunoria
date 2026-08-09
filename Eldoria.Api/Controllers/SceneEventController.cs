using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/scenes/{sceneId:int}/events")]
    [ApiController]
    public class SceneEventController(ISceneEventService sceneEventService) : ControllerBase
    {
        private readonly ISceneEventService _sceneEventService = sceneEventService;

        [HttpGet]
        public async Task<ActionResult<List<SceneEventDto>>> List(int sceneId, CancellationToken ct)
        {
            var result = await _sceneEventService.ListAsync(User.GetUserId(), sceneId, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error?.Code switch
            {
                "Auth.Forbidden" => Forbid(),
                "Scene.NotFound" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            int sceneId,
            [FromBody] CreateSceneEventRequest req,
            CancellationToken ct)
        {
            var result = await _sceneEventService.CreateAsync(User.GetUserId(), sceneId, req.Name, req.Description, ct);

            if (result.Success)
                return CreatedAtAction(nameof(List), new { sceneId }, result.Value);

            return ToError(result.Error);
        }

        [HttpPut("{eventId:int}")]
        public async Task<IActionResult> Update(
            int sceneId,
            int eventId,
            [FromBody] UpdateSceneEventRequest req,
            CancellationToken ct)
        {
            var result = await _sceneEventService.UpdateAsync(User.GetUserId(), sceneId, eventId, req.Name, req.Description, ct);

            if (result.Success)
                return Ok(result.Value);

            return ToError(result.Error);
        }

        [HttpDelete("{eventId:int}")]
        public async Task<IActionResult> Delete(int sceneId, int eventId, CancellationToken ct)
        {
            var result = await _sceneEventService.DeleteAsync(User.GetUserId(), sceneId, eventId, ct);

            if (result.Success)
                return NoContent();

            return ToError(result.Error);
        }

        [HttpPut("order")]
        public async Task<IActionResult> Reorder(
            int sceneId,
            [FromBody] ReorderSceneEventsRequest req,
            CancellationToken ct)
        {
            var events = req.Events.Select(item => (item.Id, item.SortOrder)).ToList();
            var result = await _sceneEventService.ReorderAsync(User.GetUserId(), sceneId, events, ct);

            if (result.Success)
                return NoContent();

            return ToError(result.Error);
        }

        [HttpPost("{eventId:int}/actions")]
        public async Task<IActionResult> CreateAction(
            int sceneId,
            int eventId,
            [FromBody] SceneEventActionRequest req,
            CancellationToken ct)
        {
            var result = await _sceneEventService.CreateActionAsync(
                User.GetUserId(),
                sceneId,
                eventId,
                req.Name,
                req.ActionTargetType!.Value,
                req.EventActionType!.Value,
                req.CharacterStatType!.Value,
                req.AdjustmentOperation!.Value,
                req.Value,
                req.CharacterId,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return ToError(result.Error);
        }

        [HttpPut("{eventId:int}/actions/{actionId:int}")]
        public async Task<IActionResult> UpdateAction(
            int sceneId,
            int eventId,
            int actionId,
            [FromBody] SceneEventActionRequest req,
            CancellationToken ct)
        {
            var result = await _sceneEventService.UpdateActionAsync(
                User.GetUserId(),
                sceneId,
                eventId,
                actionId,
                req.Name,
                req.ActionTargetType!.Value,
                req.EventActionType!.Value,
                req.CharacterStatType!.Value,
                req.AdjustmentOperation!.Value,
                req.Value,
                req.CharacterId,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return ToError(result.Error);
        }

        [HttpDelete("{eventId:int}/actions/{actionId:int}")]
        public async Task<IActionResult> DeleteAction(int sceneId, int eventId, int actionId, CancellationToken ct)
        {
            var result = await _sceneEventService.DeleteActionAsync(User.GetUserId(), sceneId, eventId, actionId, ct);

            if (result.Success)
                return NoContent();

            return ToError(result.Error);
        }

        [HttpPut("{eventId:int}/actions/order")]
        public async Task<IActionResult> ReorderActions(int sceneId, int eventId, [FromBody] ReorderSceneEventActionsRequest req, CancellationToken ct)
        {
            var actions = req.Actions.Select(item => (item.Id, item.SortOrder)).ToList();
            var result = await _sceneEventService.ReorderActionsAsync(User.GetUserId(), sceneId, eventId, actions, ct);

            if (result.Success)
                return NoContent();

            return ToError(result.Error);
        }

        private IActionResult ToError(Eldoria.Application.Common.Error? error) =>
            error?.Code switch
            {
                "Auth.Forbidden" => Forbid(),
                "Scene.NotFound" => BadRequest(error),
                "SceneEvent.NotFound" => BadRequest(error),
                "SceneEventAction.NotFound" => BadRequest(error),
                _ => BadRequest(error)
            };
    }
}
