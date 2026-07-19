using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class ScenePlaythroughRepository(ApplicationDbContext dbContext)
        : Repository<ScenePlaythrough>(dbContext), IScenePlaythroughRepository
    {
        private IQueryable<ScenePlaythrough> Query() => dbContext.ScenePlaythroughs
            .AsSplitQuery()
            .Include(playthrough => playthrough.Scene)
            .Include(playthrough => playthrough.Participants).ThenInclude(participant => participant.JourneyPlaythroughCharacter)
            .Include(playthrough => playthrough.Participants).ThenInclude(participant => participant.ScenePlaythroughCharacter)
            .Include(playthrough => playthrough.SceneCharacters)
            .Include(playthrough => playthrough.PlaythroughEvents).ThenInclude(playthroughEvent => playthroughEvent.SceneEvent)
            .Include(playthrough => playthrough.PlaythroughChests).ThenInclude(playthroughChest => playthroughChest.SceneChest);

        public Task<ScenePlaythrough?> GetForUserAsync(int userId, int scenePlaythroughId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(playthrough => playthrough.Id == scenePlaythroughId && playthrough.JourneyPlaythrough.Journey.UserId == userId, ct);

        public Task<ScenePlaythrough?> GetForSceneAsync(int userId, int journeyPlaythroughId, int sceneId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(playthrough => playthrough.JourneyPlaythroughId == journeyPlaythroughId && playthrough.SceneId == sceneId && playthrough.JourneyPlaythrough.Journey.UserId == userId, ct);

        public Task<List<ScenePlaythrough>> ListForPlaythroughAsync(int userId, int journeyPlaythroughId, CancellationToken ct) =>
            Query().AsNoTracking().Where(playthrough => playthrough.JourneyPlaythroughId == journeyPlaythroughId && playthrough.JourneyPlaythrough.Journey.UserId == userId)
                .OrderBy(playthrough => playthrough.Scene.SortOrder).ToListAsync(ct);

        public Task AddParticipantAsync(ScenePlaythroughParticipant participant, CancellationToken ct) =>
            dbContext.ScenePlaythroughParticipants.AddAsync(participant, ct).AsTask();

        public void RemoveParticipant(ScenePlaythroughParticipant participant) =>
            dbContext.ScenePlaythroughParticipants.Remove(participant);
    }
}
