using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface ISceneCharacterItemService
    {
        Task<Result> UseItem(int userId, int sceneCharacterItemId, CancellationToken ct);
        Task<Result> AddItem(int userId, int sceneCharacterId, int itemId, CancellationToken ct);
    }
}
