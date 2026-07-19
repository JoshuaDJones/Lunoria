using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IScenePlaythroughRepository : IRepository<ScenePlaythrough>
    {
        Task<ScenePlaythrough?> GetForUserAsync(int userId, int scenePlaythroughId, CancellationToken ct);
        Task<ScenePlaythrough?> GetForSceneAsync(int userId, int journeyPlaythroughId, int sceneId, CancellationToken ct);
        Task<List<ScenePlaythrough>> ListForPlaythroughAsync(int userId, int journeyPlaythroughId, CancellationToken ct);
        Task AddParticipantAsync(ScenePlaythroughParticipant participant, CancellationToken ct);
        void RemoveParticipant(ScenePlaythroughParticipant participant);
    }
}
