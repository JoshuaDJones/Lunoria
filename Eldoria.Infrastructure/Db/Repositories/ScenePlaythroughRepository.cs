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
            .Include(playthrough => playthrough.Participants).ThenInclude(participant => participant.JourneyPlaythroughCharacter)
            .Include(playthrough => playthrough.Participants).ThenInclude(participant => participant.ScenePlaythroughCharacter)
            .Include(playthrough => playthrough.SceneCharacters).ThenInclude(character => character.CharacterSpells)
            .Include(playthrough => playthrough.SceneCharacters).ThenInclude(character => character.ConsumableItems)
            .Include(playthrough => playthrough.SceneCharacters).ThenInclude(character => character.EquippableItems)
            .Include(playthrough => playthrough.PlaythroughEvents)
            .Include(playthrough => playthrough.PlaythroughChests);

        public Task<ScenePlaythrough?> GetForUserAsync(int userId, int scenePlaythroughId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(playthrough => playthrough.Id == scenePlaythroughId && playthrough.JourneyPlaythrough.JourneyRevision.CreatedByUserId == userId, ct);

        public Task<ScenePlaythrough?> GetForSceneAsync(int userId, int journeyPlaythroughId, int sceneId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(playthrough => playthrough.JourneyPlaythroughId == journeyPlaythroughId && playthrough.SourceSceneId == sceneId && playthrough.JourneyPlaythrough.JourneyRevision.CreatedByUserId == userId, ct);

        public Task<List<ScenePlaythrough>> ListForPlaythroughAsync(int userId, int journeyPlaythroughId, CancellationToken ct) =>
            Query().AsNoTracking().Where(playthrough => playthrough.JourneyPlaythroughId == journeyPlaythroughId && playthrough.JourneyPlaythrough.JourneyRevision.CreatedByUserId == userId)
                .OrderBy(playthrough => playthrough.SnapshotSortOrder).ToListAsync(ct);

        public Task AddParticipantAsync(ScenePlaythroughParticipant participant, CancellationToken ct) =>
            dbContext.ScenePlaythroughParticipants.AddAsync(participant, ct).AsTask();

        public void RemoveParticipant(ScenePlaythroughParticipant participant) =>
            dbContext.ScenePlaythroughParticipants.Remove(participant);
    }
}
