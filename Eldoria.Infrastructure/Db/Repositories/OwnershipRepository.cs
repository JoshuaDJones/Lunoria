using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class OwnershipRepository(ApplicationDbContext dbContext) : IOwnershipRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<Journey?> GetJourneyAsync(int userId, int journeyId, CancellationToken ct) =>
            _dbContext.Journeys.SingleOrDefaultAsync(
                journey => journey.Id == journeyId && journey.UserId == userId, ct);

        public Task<JourneyCharacter?> GetJourneyCharacterAsync(int userId, int id, CancellationToken ct) =>
            _dbContext.JourneyCharacters.SingleOrDefaultAsync(
                character => character.Id == id && character.Journey.UserId == userId, ct);

        public Task<Scene?> GetSceneAsync(int userId, int sceneId, CancellationToken ct) =>
            _dbContext.Scenes.SingleOrDefaultAsync(
                scene => scene.Id == sceneId && scene.Journey.UserId == userId, ct);

        public Task<SceneCharacter?> GetSceneCharacterAsync(int userId, int id, CancellationToken ct) =>
            _dbContext.SceneCharacters.SingleOrDefaultAsync(
                character => character.Id == id && character.Scene.Journey.UserId == userId, ct);

        public Task<JourneyPlaythrough?> GetJourneyPlaythroughAsync(int userId, int id, CancellationToken ct) =>
            _dbContext.JourneyPlaythroughs.SingleOrDefaultAsync(
                playthrough => playthrough.Id == id && playthrough.Journey.UserId == userId, ct);

        public Task<ScenePlaythrough?> GetScenePlaythroughAsync(int userId, int id, CancellationToken ct) =>
            _dbContext.ScenePlaythroughs.SingleOrDefaultAsync(
                playthrough => playthrough.Id == id && playthrough.JourneyPlaythrough.Journey.UserId == userId, ct);
    }
}
