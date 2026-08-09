using Eldoria.Api.Common;
using Eldoria.Api.Requests;
using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/scenes/{sceneId:int}/chests")]
    [ApiController]
    public class SceneChestController(ISceneChestService sceneChestService) : ControllerBase
    {
        private readonly ISceneChestService _sceneChestService = sceneChestService;

        [HttpGet]
        public async Task<IActionResult> List(
            int sceneId,
            CancellationToken ct)
        {
            var result = await _sceneChestService.ListAsync(User.GetUserId(), sceneId, ct);

            if (result.Success)
                return Ok(result.Value);

            return ToError(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            int sceneId,
            [FromBody] SceneChestRequest req,
            CancellationToken ct)
        {
            var result = await _sceneChestService.CreateAsync(
                User.GetUserId(),
                sceneId,
                req.Name,
                req.DieSides!.Value,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(List), new { sceneId }, result.Value);

            return ToError(result.Error);
        }

        [HttpPut("{sceneChestId:int}")]
        public async Task<IActionResult> Update(
            int sceneId,
            int sceneChestId,
            [FromBody] SceneChestRequest req,
            CancellationToken ct)
        {
            var result = await _sceneChestService.UpdateAsync(
                User.GetUserId(),
                sceneId,
                sceneChestId,
                req.Name,
                req.DieSides!.Value,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return ToError(result.Error);
        }

        [HttpDelete("{sceneChestId:int}")]
        public async Task<IActionResult> Delete(
            int sceneId,
            int sceneChestId,
            CancellationToken ct)
        {
            var result = await _sceneChestService.DeleteAsync(
                User.GetUserId(),
                sceneId,
                sceneChestId,
                ct);

            if (result.Success)
                return NoContent();

            return ToError(result.Error);
        }

        [HttpGet("{sceneChestId:int}/loot-entries")]
        public async Task<IActionResult> ListLootEntries(
            int sceneId,
            int sceneChestId,
            CancellationToken ct)
        {
            var result = await _sceneChestService.ListLootEntriesAsync(
                User.GetUserId(),
                sceneId,
                sceneChestId,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return ToError(result.Error);
        }

        [HttpPost("{sceneChestId:int}/loot-entries")]
        public async Task<IActionResult> CreateLootEntry(
            int sceneId,
            int sceneChestId,
            [FromBody] SceneChestLootEntryRequest req,
            CancellationToken ct)
        {
            var result = await _sceneChestService.CreateLootEntryAsync(
                User.GetUserId(),
                sceneId,
                sceneChestId,
                req.RollMinimum!.Value,
                req.RollMaximum!.Value,
                req.Quantity!.Value,
                req.EquippableItemId,
                req.ConsumableItemId,
                ct);

            if (result.Success)
                return CreatedAtAction(
                    nameof(ListLootEntries),
                    new { sceneId, sceneChestId },
                    result.Value);

            return ToError(result.Error);
        }

        [HttpPut("{sceneChestId:int}/loot-entries/{lootEntryId:int}")]
        public async Task<IActionResult> UpdateLootEntry(
            int sceneId,
            int sceneChestId,
            int lootEntryId,
            [FromBody] SceneChestLootEntryRequest req,
            CancellationToken ct)
        {
            var result = await _sceneChestService.UpdateLootEntryAsync(
                User.GetUserId(),
                sceneId,
                sceneChestId,
                lootEntryId,
                req.RollMinimum!.Value,
                req.RollMaximum!.Value,
                req.Quantity!.Value,
                req.EquippableItemId,
                req.ConsumableItemId,
                ct);

            if (result.Success)
                return Ok(result.Value);

            return ToError(result.Error);
        }

        [HttpDelete("{sceneChestId:int}/loot-entries/{lootEntryId:int}")]
        public async Task<IActionResult> DeleteLootEntry(
            int sceneId,
            int sceneChestId,
            int lootEntryId,
            CancellationToken ct)
        {
            var result = await _sceneChestService.DeleteLootEntryAsync(
                User.GetUserId(),
                sceneId,
                sceneChestId,
                lootEntryId,
                ct);

            if (result.Success)
                return NoContent();

            return ToError(result.Error);
        }

        private IActionResult ToError(Error? error) =>
            error?.Code switch
            {
                "Auth.Forbidden" => Forbid(),
                "Scene.NotFound" => BadRequest(error),
                "SceneChest.NotFound" => BadRequest(error),
                "SceneChestLootEntry.NotFound" => BadRequest(error),
                _ => BadRequest(error)
            };
    }
}
