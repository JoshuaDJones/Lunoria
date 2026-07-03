using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneProgressRepository : IRepository<SceneProgress>
    {
        Task<SceneProgress?> GetAsync(
            int userId,
            int sceneProgressId,
            CancellationToken ct);

        Task<SceneProgress?> GetForSceneAsync(
            int userId,
            int playthroughId,
            int sceneId,
            CancellationToken ct);

        Task<List<SceneProgress>> ListForPlaythroughAsync(
            int userId,
            int playthroughId,
            CancellationToken ct);

        Task AddParticipantAsync(SceneParticipant participant, CancellationToken ct);
        void RemoveParticipant(SceneParticipant participant);
        Task AddTurnAsync(SceneParticipantTurn turn, CancellationToken ct);
        void RemoveTurn(SceneParticipantTurn turn);
        Task ReorderTurnsAsync(
            IReadOnlyDictionary<int, int> positions,
            CancellationToken ct);
    }
}
