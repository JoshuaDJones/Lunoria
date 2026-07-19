using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class SceneCharacterRepository(ApplicationDbContext dbContext)
        : Repository<SceneCharacter>(dbContext), ISceneCharacterRepository
    {
        private IQueryable<SceneCharacter> Query() => dbContext.SceneCharacters
            .Include(character => character.Character)
            .Include(character => character.AlternateForm)
            .Include(character => character.SceneCharacterSpells).ThenInclude(assignment => assignment.Spell);

        public Task<List<SceneCharacter>> ListForSceneAsync(int userId, int sceneId, CancellationToken ct) =>
            Query().AsNoTracking().Where(character => character.SceneId == sceneId && character.Scene.Journey.UserId == userId)
                .OrderBy(character => character.Id).ToListAsync(ct);

        public Task<SceneCharacter?> GetForUserAsync(int userId, int sceneCharacterId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(character => character.Id == sceneCharacterId && character.Scene.Journey.UserId == userId, ct);
    }
}
