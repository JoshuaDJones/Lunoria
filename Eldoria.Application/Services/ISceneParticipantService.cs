using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface ISceneParticipantService
    {
        Task<Result<SceneParticipantDto>> AddAsync(
            int userId,
            int sceneProgressId,
            int? journeyCharacterId,
            int? sceneCharacterId,
            CancellationToken ct);

        Task<Result> RemoveAsync(
            int userId,
            int sceneProgressId,
            int participantId,
            CancellationToken ct);

        Task<Result<SceneParticipantTurnDto>> AddTurnAsync(
            int userId,
            int sceneProgressId,
            int participantId,
            int turnPosition,
            CancellationToken ct);

        Task<Result<List<SceneParticipantTurnDto>>> ReorderTurnsAsync(
            int userId,
            int sceneProgressId,
            IReadOnlyCollection<(int TurnId, int TurnPosition)> positions,
            CancellationToken ct);

        Task<Result> RemoveTurnAsync(
            int userId,
            int sceneProgressId,
            int turnId,
            CancellationToken ct);
    }
}
