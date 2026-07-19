using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneParticipantService(
        ISceneProgressRepository progressRepository,
        IOwnershipRepository ownershipRepository) : ISceneParticipantService
    {
        private readonly ISceneProgressRepository _progressRepository = progressRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;

        public async Task<Result<SceneParticipantDto>> AddAsync(
            int userId,
            int sceneProgressId,
            int? journeyCharacterId,
            int? sceneCharacterId,
            CancellationToken ct)
        {
            if (journeyCharacterId.HasValue == sceneCharacterId.HasValue)
            {
                return Fail<SceneParticipantDto>(
                    "SceneParticipant.InvalidCharacter",
                    "Exactly one journey character or scene character is required.");
            }

            var progress = await GetProgressAsync(userId, sceneProgressId, ct);
            if (progress is null)
                return NotFound<SceneParticipantDto>("SceneProgress.NotFound", "Scene progress was not found.");

            if (!progress.JourneyPlaythrough.IsActive)
                return Inactive<SceneParticipantDto>();

            if (journeyCharacterId.HasValue)
            {
                var character = await _ownershipRepository.GetJourneyCharacterAsync(
                    userId,
                    journeyCharacterId.Value,
                    ct);

                if (character is null ||
                    character.JourneyId != progress.JourneyPlaythrough.JourneyId)
                {
                    return NotFound<SceneParticipantDto>(
                        "JourneyCharacter.NotFound",
                        "Journey character was not found in the playthrough's journey.");
                }

                if (progress.Participants.Any(
                    participant => participant.JourneyCharacterId == journeyCharacterId))
                {
                    return Fail<SceneParticipantDto>(
                        "SceneParticipant.Duplicate",
                        "The journey character is already a participant.");
                }
            }
            else
            {
                var character = await _ownershipRepository.GetSceneCharacterAsync(
                    userId,
                    sceneCharacterId!.Value,
                    ct);

                if (character is null || character.SceneId != progress.SceneId)
                {
                    return NotFound<SceneParticipantDto>(
                        "SceneCharacter.NotFound",
                        "Scene character was not found in the progress scene.");
                }

                if (progress.Participants.Any(
                    participant => participant.SceneCharacterId == sceneCharacterId))
                {
                    return Fail<SceneParticipantDto>(
                        "SceneParticipant.Duplicate",
                        "The scene character is already a participant.");
                }
            }

            var participant = new SceneParticipant
            {
                SceneProgressId = sceneProgressId,
                JourneyCharacterId = journeyCharacterId,
                SceneCharacterId = sceneCharacterId,
            };

            await _progressRepository.AddParticipantAsync(participant, ct);
            await _progressRepository.SaveChangesAsync(ct);
            return Result<SceneParticipantDto>.Ok(participant.ToDto());
        }

        public async Task<Result> RemoveAsync(
            int userId,
            int sceneProgressId,
            int participantId,
            CancellationToken ct)
        {
            var progress = await GetProgressAsync(userId, sceneProgressId, ct);
            if (progress is null)
                return Result.Fail(new Error("SceneProgress.NotFound", "Scene progress was not found."));

            if (!progress.JourneyPlaythrough.IsActive)
                return Result.Fail(InactiveError());

            var participant = progress.Participants.SingleOrDefault(
                participant => participant.Id == participantId);

            if (participant is null)
            {
                return Result.Fail(new Error(
                    "SceneParticipant.NotFound",
                    "Scene participant was not found."));
            }

            if (participant.Turns.Count > 0)
            {
                return Result.Fail(new Error(
                    "SceneParticipant.HasTurns",
                    "Remove the participant's turns before removing the participant."));
            }

            _progressRepository.RemoveParticipant(participant);
            await _progressRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result<SceneParticipantTurnDto>> AddTurnAsync(
            int userId,
            int sceneProgressId,
            int participantId,
            int turnPosition,
            CancellationToken ct)
        {
            var progress = await GetProgressAsync(userId, sceneProgressId, ct);
            if (progress is null)
                return NotFound<SceneParticipantTurnDto>("SceneProgress.NotFound", "Scene progress was not found.");

            if (!progress.JourneyPlaythrough.IsActive)
                return Inactive<SceneParticipantTurnDto>();

            if (progress.Participants.All(participant => participant.Id != participantId))
            {
                return NotFound<SceneParticipantTurnDto>(
                    "SceneParticipant.NotFound",
                    "Scene participant was not found in this progress record.");
            }

            if (progress.ParticipantTurns.Any(turn => turn.TurnPosition == turnPosition))
            {
                return Fail<SceneParticipantTurnDto>(
                    "SceneParticipantTurn.PositionConflict",
                    "The turn position is already in use.");
            }

            var turn = new SceneParticipantTurn
            {
                SceneProgressId = sceneProgressId,
                SceneParticipantId = participantId,
                TurnPosition = turnPosition,
            };

            await _progressRepository.AddTurnAsync(turn, ct);
            await _progressRepository.SaveChangesAsync(ct);
            return Result<SceneParticipantTurnDto>.Ok(turn.ToDto());
        }

        public async Task<Result<List<SceneParticipantTurnDto>>> ReorderTurnsAsync(
            int userId,
            int sceneProgressId,
            IReadOnlyCollection<(int TurnId, int TurnPosition)> positions,
            CancellationToken ct)
        {
            var progress = await GetProgressAsync(userId, sceneProgressId, ct);
            if (progress is null)
                return NotFound<List<SceneParticipantTurnDto>>("SceneProgress.NotFound", "Scene progress was not found.");

            if (!progress.JourneyPlaythrough.IsActive)
                return Inactive<List<SceneParticipantTurnDto>>();

            var turnIds = positions.Select(position => position.TurnId).ToList();
            var requestedPositions = positions.Select(position => position.TurnPosition).ToList();

            if (turnIds.Count != turnIds.Distinct().Count() ||
                requestedPositions.Count != requestedPositions.Distinct().Count())
            {
                return Fail<List<SceneParticipantTurnDto>>(
                    "SceneParticipantTurn.InvalidOrder",
                    "Turn IDs and positions must be unique.");
            }

            var existingTurnIds = progress.ParticipantTurns
                .Select(turn => turn.Id)
                .OrderBy(id => id)
                .ToList();

            if (!existingTurnIds.SequenceEqual(turnIds.OrderBy(id => id)))
            {
                return Fail<List<SceneParticipantTurnDto>>(
                    "SceneParticipantTurn.InvalidOrder",
                    "The reorder request must contain every turn in the progress record exactly once.");
            }

            var expectedPositions = Enumerable.Range(1, positions.Count).ToList();
            if (!expectedPositions.SequenceEqual(requestedPositions.OrderBy(position => position)))
            {
                return Fail<List<SceneParticipantTurnDto>>(
                    "SceneParticipantTurn.InvalidOrder",
                    "Turn positions must form a contiguous sequence beginning at 1.");
            }

            var positionMap = positions.ToDictionary(
                position => position.TurnId,
                position => position.TurnPosition);

            await _progressRepository.ReorderTurnsAsync(positionMap, ct);

            foreach (var turn in progress.ParticipantTurns)
                turn.TurnPosition = positionMap[turn.Id];

            return Result<List<SceneParticipantTurnDto>>.Ok(
                progress.ParticipantTurns
                    .OrderBy(turn => turn.TurnPosition)
                    .Select(turn => turn.ToDto())
                    .ToList());
        }

        public async Task<Result> RemoveTurnAsync(
            int userId,
            int sceneProgressId,
            int turnId,
            CancellationToken ct)
        {
            var progress = await GetProgressAsync(userId, sceneProgressId, ct);
            if (progress is null)
                return Result.Fail(new Error("SceneProgress.NotFound", "Scene progress was not found."));

            if (!progress.JourneyPlaythrough.IsActive)
                return Result.Fail(InactiveError());

            var turn = progress.ParticipantTurns.SingleOrDefault(turn => turn.Id == turnId);
            if (turn is null)
            {
                return Result.Fail(new Error(
                    "SceneParticipantTurn.NotFound",
                    "Scene participant turn was not found."));
            }

            _progressRepository.RemoveTurn(turn);
            await _progressRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        private Task<ScenePlaythrough?> GetProgressAsync(
            int userId,
            int sceneProgressId,
            CancellationToken ct) =>
            _progressRepository.GetAsync(userId, sceneProgressId, ct);

        private static Result<T> NotFound<T>(string code, string message) =>
            Result<T>.Fail(new Error(code, message));

        private static Result<T> Fail<T>(string code, string message) =>
            Result<T>.Fail(new Error(code, message));

        private static Result<T> Inactive<T>() =>
            Result<T>.Fail(InactiveError());

        private static Error InactiveError() =>
            new(
                "SceneProgress.PlaythroughInactive",
                "Participants and turns cannot be changed after the playthrough is inactive.");
    }
}
