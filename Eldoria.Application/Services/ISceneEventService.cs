using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Enums;

namespace Eldoria.Application.Services
{

    public interface ISceneEventService
    {
        Task<Result<List<SceneEventDto>>> ListAsync(int userId, int sceneId, CancellationToken ct);
        Task<Result<SceneEventDto>> CreateAsync(int userId, int sceneId, string name, string? description, CancellationToken ct);
        Task<Result<SceneEventDto>> UpdateAsync(int userId, int sceneId, int eventId, string name, string? description, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int sceneId, int eventId, CancellationToken ct);
        Task<Result> ReorderAsync(int userId, int sceneId, IReadOnlyList<(int Id, int SortOrder)> events, CancellationToken ct);
        Task<Result<SceneEventActionDto>> CreateActionAsync(int userId, int sceneId, int eventId, string name, ActionTargetType targetType, EventActionType actionType, CharacterStatType statType, AdjustmentOperation operation, int value, int? characterId, CancellationToken ct);
        Task<Result<SceneEventActionDto>> UpdateActionAsync(int userId, int sceneId, int eventId, int actionId, string name, ActionTargetType targetType, EventActionType actionType, CharacterStatType statType, AdjustmentOperation operation, int value, int? characterId, CancellationToken ct);
        Task<Result> DeleteActionAsync(int userId, int sceneId, int eventId, int actionId, CancellationToken ct);
        Task<Result> ReorderActionsAsync(int userId, int sceneId, int eventId, IReadOnlyList<(int Id, int SortOrder)> actions, CancellationToken ct);
    }
}
