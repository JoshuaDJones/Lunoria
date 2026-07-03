using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IOwnershipRepository
    {
        Task<Journey?> GetJourneyAsync(int userId, int journeyId, CancellationToken ct);
        Task<JourneyCharacter?> GetJourneyCharacterAsync(int userId, int journeyCharacterId, CancellationToken ct);
        Task<JourneyCharacterItem?> GetJourneyCharacterItemAsync(int userId, int journeyCharacterItemId, CancellationToken ct);
        Task<Scene?> GetSceneAsync(int userId, int sceneId, CancellationToken ct);
        Task<SceneCharacter?> GetSceneCharacterAsync(int userId, int sceneCharacterId, CancellationToken ct);
        Task<SceneCharacterItem?> GetSceneCharacterItemAsync(int userId, int sceneCharacterItemId, CancellationToken ct);
    }
}
