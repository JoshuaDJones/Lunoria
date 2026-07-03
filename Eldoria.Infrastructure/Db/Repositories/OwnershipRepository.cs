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

        public Task<JourneyCharacterItem?> GetJourneyCharacterItemAsync(int userId, int id, CancellationToken ct) =>
            _dbContext.JourneyCharacterItems.SingleOrDefaultAsync(
                item => item.Id == id && item.JourneyCharacter.Journey.UserId == userId, ct);

        public Task<Scene?> GetSceneAsync(int userId, int sceneId, CancellationToken ct) =>
            _dbContext.Scenes.SingleOrDefaultAsync(
                scene => scene.Id == sceneId && scene.Journey.UserId == userId, ct);

        public Task<SceneCharacter?> GetSceneCharacterAsync(int userId, int id, CancellationToken ct) =>
            _dbContext.SceneCharacters.SingleOrDefaultAsync(
                character => character.Id == id && character.Scene.Journey.UserId == userId, ct);

        public Task<SceneCharacterItem?> GetSceneCharacterItemAsync(int userId, int id, CancellationToken ct) =>
            _dbContext.SceneCharacterItems.SingleOrDefaultAsync(
                item => item.Id == id && item.SceneCharacter.Scene.Journey.UserId == userId, ct);
    }
}
