using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface ISceneCharacterItemService
    {
        Task<Result> UseItem(int sceneCharacterItemId, CancellationToken ct);
        Task<Result> AddItem(int sceneCharacterId, int itemId, CancellationToken ct);
    }
}
