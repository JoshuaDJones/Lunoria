using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyCharacterRepository : Repository<JourneyCharacter>, IJourneyCharacterRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public JourneyCharacterRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<JourneyCharacter>> GetJourneyCharacters(int journeyId, CancellationToken ct)
        {
            return await _dbContext.JourneyCharacters
                .Include(jc => jc.Character)
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.AlternateForm)
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.CharacterSpells)
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.CharacterSpells)
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.CharacterSpells)
                        .ThenInclude(cs => cs.Spell)
                .Include(jc => jc.JourneyCharacterItems)
                .Include(jc => jc.JourneyCharacterItems)
                    .ThenInclude(jci => jci.Item)
                .Where(jc => jc.JourneyId == journeyId)
                .ToListAsync(ct);
        }
    }
}
