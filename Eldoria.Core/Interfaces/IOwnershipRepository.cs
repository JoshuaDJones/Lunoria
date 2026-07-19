using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IOwnershipRepository
    {
        Task<Journey?> GetJourneyAsync(int userId, int journeyId, CancellationToken ct);
        Task<JourneyCharacter?> GetJourneyCharacterAsync(int userId, int journeyCharacterId, CancellationToken ct);
        Task<Scene?> GetSceneAsync(int userId, int sceneId, CancellationToken ct);
        Task<SceneCharacter?> GetSceneCharacterAsync(int userId, int sceneCharacterId, CancellationToken ct);
        Task<JourneyPlaythrough?> GetJourneyPlaythroughAsync(int userId, int journeyPlaythroughId, CancellationToken ct);
        Task<ScenePlaythrough?> GetScenePlaythroughAsync(int userId, int scenePlaythroughId, CancellationToken ct);
    }
}
