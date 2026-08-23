using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface IScenePlaythroughService
{
    Task<Result> StartAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<Result<ScenePlaythroughDetailsDto>> GetAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<Result> AddSceneCharacterInstanceAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int scenePlaythroughCharacterId,
        CancellationToken ct);

    Task<Result> ActivateJourneyCharacterAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int journeyPlaythroughCharacterId,
        CancellationToken ct);

    Task<Result> AddPlaythroughCharacterAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int playthroughCharacterId,
        CancellationToken ct);

    Task<Result> UpdateParticipantStatsAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        SceneParticipantStatsUpdateDto update,
        CancellationToken ct);

    Task<Result<SceneMovementResultDto>> RecordMovementAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        int roll,
        CancellationToken ct);

    Task<Result> ForfeitActionAsync(
        int userId,
        int playthroughId,
        int sceneId,
        int participantId,
        CancellationToken ct);
}
