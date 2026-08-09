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
    public class SceneCharacterController(ISceneCharacterService sceneCharacterService) : ControllerBase
    {
        private readonly ISceneCharacterService _sceneCharacterService = sceneCharacterService;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int sceneId, CancellationToken ct)
        {
            var result = await _sceneCharacterService.ListAsync(User.GetUserId(), sceneId, ct);
            return result.Success ? Ok(result.Value) : ToError(result.Error);
        }

        [HttpGet("{sceneCharacterId:int}")]
        public async Task<IActionResult> Get(int sceneCharacterId, CancellationToken ct)
        {
            var result = await _sceneCharacterService.GetAsync(User.GetUserId(), sceneCharacterId, ct);
            return result.Success ? Ok(result.Value) : ToError(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> AddSceneCharacter(
            [FromBody] AddSceneCharacterRequest req,
            CancellationToken ct)
        {
            var result = await _sceneCharacterService.AddSceneCharacterAsync(
                User.GetUserId(),
                req.SceneId!.Value,
                req.CharacterId!.Value,
                ct);

            if (result.Success)
                return CreatedAtAction(nameof(Get), new { sceneCharacterId = result.Value!.Id }, result.Value);

            return ToError(result.Error);
        }

        [HttpPut("{sceneCharacterId:int}")]
        public async Task<IActionResult> Update(
            int sceneCharacterId,
            [FromBody] UpdateSceneCharacterRequest req,
            CancellationToken ct)
        {
            var result = await _sceneCharacterService.UpdateAsync(
                User.GetUserId(),
                sceneCharacterId,
                req.MeleeAttackDamage,
                req.BowAttackDamage,
                req.Movement!.Value,
                req.MaxConsumableInventory!.Value,
                req.MaxEquippableInventory!.Value,
                req.MaxHp!.Value,
                req.MaxMp!.Value,
                req.IsInitiallyActive!.Value,
                req.AlternateFormId,
                ct);

            return result.Success ? Ok(result.Value) : ToError(result.Error);
        }

        [HttpPut("{sceneCharacterId:int}/spells")]
        public async Task<IActionResult> ReplaceSpells(
            int sceneCharacterId,
            [FromBody] ReplaceSceneCharacterSpellsRequest req,
            CancellationToken ct)
        {
            var result = await _sceneCharacterService.ReplaceSpellsAsync(
                User.GetUserId(),
                sceneCharacterId,
                req.SpellIds,
                ct);

            return result.Success ? Ok(result.Value) : ToError(result.Error);
        }

        [HttpDelete("{sceneCharacterId:int}")]
        public async Task<IActionResult> Delete(int sceneCharacterId, CancellationToken ct)
        {
            var result = await _sceneCharacterService.DeleteSceneCharacterAsync(
                User.GetUserId(),
                sceneCharacterId,
                ct);

            return result.Success ? NoContent() : ToError(result.Error);
        }

        private IActionResult ToError(Error? error) =>
            error?.Code switch
            {
                "Auth.Forbidden" => Forbid(),
                "SceneCharacter.NotFound" => NotFound(error),
                "Scene.NotFound" => BadRequest(error),
                "Character.NotFound" => BadRequest(error),
                _ => BadRequest(error)
            };
    }
}
