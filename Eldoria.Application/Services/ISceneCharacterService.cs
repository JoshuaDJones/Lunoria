using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface ISceneCharacterService
    {
        Task<Result> AddSceneCharacterAsync(int userId, int sceneId, int characterId, CancellationToken ct);
        Task<Result> DeleteSceneCharacterAsync(int userId, int sceneCharacterId, CancellationToken ct);
        Task<Result> AdjustCharacterHpMpAsync(int userId, int sceneCharacterId, int newHp, int newMp, CancellationToken ct);
    }
}
