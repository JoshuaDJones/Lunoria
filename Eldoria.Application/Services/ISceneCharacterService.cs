using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface ISceneCharacterService
    {
        Task<Result> AddSceneCharacterAsync(int sceneId, int characterId, CancellationToken ct);
        Task<Result> DeleteSceneCharacterAsync(int sceneCharacterId, CancellationToken ct);
        Task<Result> AdjustCharacterHpMpAsync(int sceneCharacterId, int newHp, int newMp, CancellationToken ct);
    }
}
