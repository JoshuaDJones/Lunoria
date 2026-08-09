using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneEventService(
        ISceneEventRepository sceneEventRepository,
        IRepository<Scene> sceneRepository,
        IRepository<Journey> journeyRepository,
        IRepository<SceneEventAction> sceneEventActionRepository,
        IJourneyCharacterRepository journeyCharacterRepository) : ISceneEventService
    {
        private readonly ISceneEventRepository _sceneEventRepository = sceneEventRepository;
        private readonly IRepository<Scene> _sceneRepository = sceneRepository;
        private readonly IRepository<Journey> _journeyRepository = journeyRepository;
        private readonly IRepository<SceneEventAction> _sceneEventActionRepository = sceneEventActionRepository;
        private readonly IJourneyCharacterRepository _journeyCharacterRepository = journeyCharacterRepository;

        public async Task<Result<List<SceneEventDto>>> ListAsync(
            int userId,
            int sceneId,
            CancellationToken ct)
        {
            var (_, error) = await GetOwnedSceneAsync(userId, sceneId, ct);

            if (error is not null)
                return Result<List<SceneEventDto>>.Fail(error);

            var events = await _sceneEventRepository.ListForSceneAsync(userId, sceneId, ct);

            return Result<List<SceneEventDto>>.Ok(events.Select(item => item.ToDto()).ToList());
        }

        public async Task<Result<SceneEventDto>> CreateAsync(
            int userId,
            int sceneId,
            string name,
            string? description,
            CancellationToken ct)
        {
            var (_, error) = await GetOwnedSceneAsync(userId, sceneId, ct);

            if (error is not null)
                return Result<SceneEventDto>.Fail(error);

            var sceneEvent = new SceneEvent
            {
                SceneId = sceneId,
                Name = name.Trim(),
                Description = description?.Trim()
            };

            await _sceneEventRepository.AddWithNextSortOrderAsync(sceneEvent, ct);

            return Result<SceneEventDto>.Ok(sceneEvent.ToDto());
        }

        public async Task<Result<SceneEventDto>> UpdateAsync(
            int userId,
            int sceneId,
            int eventId,
            string name,
            string? description,
            CancellationToken ct)
        {
            var sceneEvent = await GetOwnedEventAsync(userId, sceneId, eventId, ct);

            if (sceneEvent is null)
                return Result<SceneEventDto>.Fail(NotFoundEvent);

            sceneEvent.Name = name.Trim();
            sceneEvent.Description = description?.Trim();

            await _sceneEventRepository.SaveChangesAsync(ct);

            return Result<SceneEventDto>.Ok(sceneEvent.ToDto());
        }

        public async Task<Result> DeleteAsync(
            int userId,
            int sceneId,
            int eventId,
            CancellationToken ct)
        {
            var sceneEvent = await GetOwnedEventAsync(userId, sceneId, eventId, ct);

            if (sceneEvent is null)
                return Result.Fail(NotFoundEvent);

            _sceneEventRepository.Remove(sceneEvent);
            await _sceneEventRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> ReorderAsync(
            int userId,
            int sceneId,
            IReadOnlyList<(int Id, int SortOrder)> events,
            CancellationToken ct)
        {
            var (_, error) = await GetOwnedSceneAsync(userId, sceneId, ct);

            if (error is not null)
                return Result.Fail(error);

            if (!HasValidOrder(events))
                return Result.Fail(InvalidOrder("event"));

            var sortOrders = events.ToDictionary(item => item.Id, item => item.SortOrder);
            var reordered = await _sceneEventRepository.ReorderAsync(sceneId, sortOrders, ct);

            return reordered
                ? Result.Ok()
                : Result.Fail(InvalidOrder("event"));
        }

        public async Task<Result<SceneEventActionDto>> CreateActionAsync(
            int userId,
            int sceneId,
            int eventId,
            string name,
            ActionTargetType targetType,
            EventActionType actionType,
            CharacterStatType statType,
            AdjustmentOperation operation,
            int value,
            int? characterId,
            CancellationToken ct)
        {
            var sceneEvent = await GetOwnedEventAsync(userId, sceneId, eventId, ct);

            if (sceneEvent is null)
                return Result<SceneEventActionDto>.Fail(NotFoundEvent);

            var validationError = await ValidateActionAsync(userId, sceneEvent.SceneId, targetType, actionType, statType, operation, characterId, ct);

            if (validationError is not null)
                return Result<SceneEventActionDto>.Fail(validationError);

            var action = BuildAction(eventId, name, targetType, actionType, statType, operation, value, characterId);

            await _sceneEventRepository.AddActionWithNextSortOrderAsync(action, ct);

            return Result<SceneEventActionDto>.Ok(action.ToDto());
        }

        public async Task<Result<SceneEventActionDto>> UpdateActionAsync(
            int userId,
            int sceneId,
            int eventId,
            int actionId,
            string name,
            ActionTargetType targetType,
            EventActionType actionType,
            CharacterStatType statType,
            AdjustmentOperation operation,
            int value,
            int? characterId,
            CancellationToken ct)
        {
            var sceneEvent = await GetOwnedEventAsync(userId, sceneId, eventId, ct);

            if (sceneEvent is null)
                return Result<SceneEventActionDto>.Fail(NotFoundEvent);

            var action = await _sceneEventRepository.GetActionForUserAsync(userId, actionId, ct);

            if (action is null || action.SceneEventId != eventId)
                return Result<SceneEventActionDto>.Fail(NotFoundAction);

            var validationError = await ValidateActionAsync(userId, sceneEvent.SceneId, targetType, actionType, statType, operation, characterId, ct);

            if (validationError is not null)
                return Result<SceneEventActionDto>.Fail(validationError);

            action.Name = name.Trim();
            action.ActionTargetType = targetType;
            action.EventActionType = actionType;
            action.CharacterStatAdjustmentAction ??= new CharacterStatAdjustmentAction();
            SetAdjustment(action.CharacterStatAdjustmentAction, statType, operation, value, characterId);

            await _sceneEventRepository.SaveChangesAsync(ct);

            return Result<SceneEventActionDto>.Ok(action.ToDto());
        }

        public async Task<Result> DeleteActionAsync(
            int userId,
            int sceneId,
            int eventId,
            int actionId,
            CancellationToken ct)
        {
            if (await GetOwnedEventAsync(userId, sceneId, eventId, ct) is null)
                return Result.Fail(NotFoundEvent);

            var action = await _sceneEventRepository.GetActionForUserAsync(userId, actionId, ct);

            if (action is null || action.SceneEventId != eventId)
                return Result.Fail(NotFoundAction);

            _sceneEventActionRepository.Remove(action);
            await _sceneEventActionRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> ReorderActionsAsync(
            int userId,
            int sceneId,
            int eventId,
            IReadOnlyList<(int Id, int SortOrder)> actions,
            CancellationToken ct)
        {
            if (await GetOwnedEventAsync(userId, sceneId, eventId, ct) is null)
                return Result.Fail(NotFoundEvent);

            if (!HasValidOrder(actions))
                return Result.Fail(InvalidOrder("action"));

            var sortOrders = actions.ToDictionary(item => item.Id, item => item.SortOrder);
            var reordered = await _sceneEventRepository.ReorderActionsAsync(eventId, sortOrders, ct);

            return reordered
                ? Result.Ok()
                : Result.Fail(InvalidOrder("action"));
        }

        private async Task<(Scene? Scene, Error? Error)> GetOwnedSceneAsync(int userId, int sceneId, CancellationToken ct)
        {
            var scene = await _sceneRepository.GetByIdAsync(sceneId, ct);

            if (scene is null)
                return (null, new Error("Scene.NotFound", "The scene does not exist."));

            var journey = await _journeyRepository.GetByIdAsync(scene.JourneyId, ct);

            return journey?.UserId == userId
                ? (scene, null)
                : (null, new Error("Auth.Forbidden", "You do not have permission to modify this scene."));
        }

        private async Task<SceneEvent?> GetOwnedEventAsync(int userId, int sceneId, int eventId, CancellationToken ct)
        {
            var sceneEvent = await _sceneEventRepository.GetForUserAsync(userId, eventId, ct);
            return sceneEvent?.SceneId == sceneId ? sceneEvent : null;
        }

        private async Task<Error?> ValidateActionAsync(int userId, int sceneId, ActionTargetType targetType, EventActionType actionType, CharacterStatType statType, AdjustmentOperation operation, int? characterId, CancellationToken ct)
        {
            if (!Enum.IsDefined(targetType) || !Enum.IsDefined(actionType) || !Enum.IsDefined(statType) || !Enum.IsDefined(operation))
                return new Error("SceneEventAction.InvalidType", "One or more action values are invalid.");
            if (actionType != EventActionType.CharacterStatAdjustment)
                return new Error("SceneEventAction.UnsupportedType", "The action type is not supported.");
            if (targetType == ActionTargetType.AllJourneyCharacters && characterId is not null)
                return new Error("SceneEventAction.InvalidTarget", "A character cannot be supplied when targeting all journey characters.");
            if (targetType == ActionTargetType.SingleJourneyCharacter && characterId is null)
                return new Error("SceneEventAction.InvalidTarget", "A character is required for a single-character target.");
            if (characterId is not null)
            {
                var (scene, error) = await GetOwnedSceneAsync(userId, sceneId, ct);
                if (error is not null) return error;
                var characters = await _journeyCharacterRepository.GetJourneyCharacters(scene!.JourneyId, ct);
                if (!characters.Any(item => item.CharacterId == characterId))
                    return new Error("SceneEventAction.CharacterNotFound", "The character is not attached to this journey.");
            }
            return null;
        }

        private static SceneEventAction BuildAction(int eventId, string name, ActionTargetType targetType, EventActionType actionType, CharacterStatType statType, AdjustmentOperation operation, int value, int? characterId)
        {
            var action = new SceneEventAction { SceneEventId = eventId, Name = name.Trim(), ActionTargetType = targetType, EventActionType = actionType };
            action.CharacterStatAdjustmentAction = new CharacterStatAdjustmentAction();
            SetAdjustment(action.CharacterStatAdjustmentAction, statType, operation, value, characterId);
            return action;
        }

        private static void SetAdjustment(CharacterStatAdjustmentAction adjustment, CharacterStatType statType, AdjustmentOperation operation, int value, int? characterId)
        {
            adjustment.CharacterStatType = statType;
            adjustment.AdjustmentOperation = operation;
            adjustment.Value = value;
            adjustment.CharacterId = characterId;
        }

        private static bool HasValidOrder(IReadOnlyList<(int Id, int SortOrder)> items) =>
            items.Count > 0 && items.Select(item => item.Id).Distinct().Count() == items.Count &&
            !items.Select(item => item.SortOrder).OrderBy(order => order).Where((order, index) => order != index).Any();

        private static Error InvalidOrder(string item) => new("SceneEvent.InvalidOrder", $"The submitted {item} IDs must be unique, complete, and ordered contiguously from zero.");
        private static readonly Error NotFoundEvent = new("SceneEvent.NotFound", "The scene event does not exist.");
        private static readonly Error NotFoundAction = new("SceneEventAction.NotFound", "The scene event action does not exist.");
    }
}
