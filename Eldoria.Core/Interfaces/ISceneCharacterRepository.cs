using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneCharacterRepository : IRepository<SceneCharacter>
    {
        Task<List<SceneCharacter>> ListForSceneAsync(int userId, int sceneId, CancellationToken ct);
        Task<SceneCharacter?> GetForUserAsync(int userId, int sceneCharacterId, CancellationToken ct);
    }
}
