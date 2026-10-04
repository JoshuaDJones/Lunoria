using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class CharacterUsageRepository(ApplicationDbContext db) : ICharacterUsageRepository
{
    // Check all authoring assignments, including filtered parents; never runtime snapshots.
    public async Task<bool> HasDirectAssignmentsAsync(int characterId, CancellationToken ct) =>
        await db.JourneyCharacters.IgnoreQueryFilters().AnyAsync(c => c.CharacterId == characterId, ct) ||
        await db.SceneCharacters.IgnoreQueryFilters().AnyAsync(c => c.CharacterId == characterId, ct);
}

