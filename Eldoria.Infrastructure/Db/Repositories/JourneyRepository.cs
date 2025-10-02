using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyRepository : Repository<Journey>, IJourneyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public JourneyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Journey>> GetUsersJourneys(int userId, int skip, int take, CancellationToken ct)
        {
            var query = _dbContext.Journeys
                .AsNoTracking()
                .Where(j => j.UserId == userId);

            return await query
                .OrderBy(j => j.CreateDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public async Task<Journey?> GetJourneyWithPlayers(int journeyId, CancellationToken ct)
        {
            return await _dbContext.Journeys
                .AsNoTracking()
                .Include(j => j.Scenes)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.AlternateForm)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                            .ThenInclude(ch => ch.Spell)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.JourneyCharacterItems)
                        .ThenInclude(jci => jci.Item)
                .FirstOrDefaultAsync(j => j.Id == journeyId, ct);
        }
    }
}
