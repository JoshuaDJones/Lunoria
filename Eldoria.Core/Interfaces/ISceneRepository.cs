using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneRepository : IRepository<Scene>
    {
        Task<List<Scene>> GetJourneyScenes(int journeyId, int skip, int take, CancellationToken ct);
    }
}
