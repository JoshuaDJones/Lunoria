using Eldoria.Api.Requests;
using Eldoria.Application.Services;
using Eldoria.Api.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Eldoria.Application.Dtos;
using Eldoria.Application.Common;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JourneyCharacterController(IJourneyCharacterService journeyCharacterService, ICharacterSyncService characterSyncService) : ControllerBase
    {
        private readonly IJourneyCharacterService _journeyCharacterService = journeyCharacterService;

        [Authorize]
        [HttpGet("{assignmentId:int}/sync-preview")]
        public async Task<IActionResult> ReviewBase(int assignmentId, CancellationToken ct) =>
            SyncResponse(await characterSyncService.ReviewAsync(User.GetUserId(), assignmentId, false, ct));

        [Authorize]
        [HttpPost("{assignmentId:int}/sync")]
        public async Task<IActionResult> SyncBase(int assignmentId, [FromBody] SyncCharacterFromBaseRequest request, CancellationToken ct) =>
            SyncResponse(await characterSyncService.ApplyAsync(User.GetUserId(), assignmentId, false, request.ToSelection(), false, ct));

        [Authorize]
        [HttpPost("{assignmentId:int}/acknowledge-base-changes")]
        public async Task<IActionResult> AcknowledgeBase(int assignmentId, [FromBody] AcknowledgeCharacterBaseChangesRequest request, CancellationToken ct) =>
            SyncResponse(await characterSyncService.ApplyAsync(User.GetUserId(), assignmentId, false, request.ToSelection(), true, ct));        

        [HttpPut("{journeyId:int}")]
        public async Task<IActionResult> Replace(int journeyId, [FromBody] ReplaceJourneyCharactersRequest req, CancellationToken ct)
        {
            var result = await _journeyCharacterService.ReplaceJourneyCharacters(User.GetUserId(), journeyId, req.CharacterIds, ct);

            if (result.Success)
                return Ok();

            return result.Error?.Code switch
            {
                "Journey.NotFound" => BadRequest(result.Error),
                "Character.NotFound" => BadRequest(result.Error),
                "JourneyCharacter.InUse" => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpDelete("{journeyCharacterId:int}")]
        public async Task<IActionResult> Delete(int journeyCharacterId, CancellationToken ct)
        {
            var result = await _journeyCharacterService.DeleteAsync(User.GetUserId(), journeyCharacterId, ct);

            if (result.Success) 
                return Ok();

            return result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => BadRequest(result?.Error),
                _ => BadRequest(result?.Error)
            };
        }

        [HttpPut("assignment/{journeyCharacterId:int}")]
        public async Task<IActionResult> Update(
            int journeyCharacterId,
            [FromBody] UpdateJourneyCharacterStatsRequest req,
            CancellationToken ct)
        {
            var result = await _journeyCharacterService.UpdateAsync(
                User.GetUserId(), journeyCharacterId,
                req.MeleeAttackDamage, req.BowAttackDamage,
                req.Movement!.Value, req.MaxConsumableInventory!.Value,
                req.MaxEquippableInventory!.Value, req.MaxHp!.Value,
                req.MaxMp!.Value, req.IsInitiallyActive!.Value,
                req.AlternateFormId, ct, req.SortOrder);

            return result.Success ? Ok(result.Value) : result.Error?.Code switch
            {
                "JourneyCharacter.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        private IActionResult SyncResponse(Result<CharacterSyncPreviewDto> result) =>
            result.Success ? Ok(result.Value) : result.Error.Code switch
            {
                "CharacterSync.NotFound" => NotFound(result.Error),
                "CharacterSync.Conflict" => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
    }
}
