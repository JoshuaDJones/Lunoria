using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class PlaythroughJoinSessionRepository(ApplicationDbContext dbContext)
    : IPlaythroughJoinSessionRepository
{
    public Task<Playthrough?> GetOwnedPlaythroughAsync(
        int userId,
        int playthroughId,
        CancellationToken ct)
    {
        return dbContext.Playthroughs.SingleOrDefaultAsync(
            playthrough =>
                playthrough.Id == playthroughId &&
                playthrough.UserId == userId,
            ct);
    }

    public Task<PlaythroughJoinSession?> GetForOwnerAsync(
        int userId,
        int playthroughId,
        CancellationToken ct)
    {
        return dbContext.PlaythroughJoinSessions
            .Include(session => session.Playthrough)
            .SingleOrDefaultAsync(
                session =>
                    session.PlaythroughId == playthroughId &&
                    session.Playthrough.UserId == userId,
                ct);
    }

    public Task<PlaythroughJoinSession?> GetValidByTokenHashAsync(
        string tokenHash,
        DateTime now,
        CancellationToken ct)
    {
        return dbContext.PlaythroughJoinSessions
            .AsNoTracking()
            .Include(session => session.Playthrough)
            .SingleOrDefaultAsync(
                session =>
                    session.TokenHash == tokenHash &&
                    session.RevokedAt == null &&
                    session.ExpiresAt > now &&
                    session.Playthrough.CompletedAt == null,
                ct);
    }

    public Task<Playthrough?> GetPublicSnapshotAsync(
        string tokenHash,
        DateTime now,
        CancellationToken ct)
    {
        return dbContext.Playthroughs
            .AsNoTrackingWithIdentityResolution()
            .AsSplitQuery()
            .Where(playthrough =>
                playthrough.CompletedAt == null &&
                playthrough.JoinSession != null &&
                playthrough.JoinSession.TokenHash == tokenHash &&
                playthrough.JoinSession.RevokedAt == null &&
                playthrough.JoinSession.ExpiresAt > now)
            .Include(playthrough => playthrough.EventLogs)
            .Include(playthrough => playthrough.JourneyCharacters)
                .ThenInclude(character => character.PlaythroughCharacter)
            .Include(playthrough => playthrough.JourneyCharacters)
                .ThenInclude(character => character.AlternateForm)
                    .ThenInclude(alternate => alternate!.Spells)
                        .ThenInclude(link => link.PlaythroughSpell)
                            .ThenInclude(spell => spell.PlaythroughSpellType)
            .Include(playthrough => playthrough.JourneyCharacters)
                .ThenInclude(character => character.Spells)
                    .ThenInclude(link => link.PlaythroughSpell)
                        .ThenInclude(spell => spell.PlaythroughSpellType)
            .Include(playthrough => playthrough.JourneyCharacters)
                .ThenInclude(character => character.ConsumableItems)
                    .ThenInclude(link => link.PlaythroughConsumableItem)
            .Include(playthrough => playthrough.JourneyCharacters)
                .ThenInclude(character => character.EquippableItems)
                    .ThenInclude(link => link.PlaythroughEquippableItem)
                        .ThenInclude(item => item.AffectedSpellType)
            .Include(playthrough => playthrough.JourneyCharacters)
                .ThenInclude(character => character.EquippableItems)
                    .ThenInclude(link => link.PlaythroughEquippableItem)
                        .ThenInclude(item => item.AddedSpells)
                            .ThenInclude(spell => spell.PlaythroughSpellType)
            .Include(playthrough => playthrough.Scenes)
                .ThenInclude(scene => scene.SceneCharacters)
                    .ThenInclude(character => character.PlaythroughCharacter)
            .Include(playthrough => playthrough.Scenes)
                .ThenInclude(scene => scene.SceneCharacters)
                    .ThenInclude(character => character.AlternateForm)
                        .ThenInclude(alternate => alternate!.Spells)
                            .ThenInclude(link => link.PlaythroughSpell)
                                .ThenInclude(spell => spell.PlaythroughSpellType)
            .Include(playthrough => playthrough.Scenes)
                .ThenInclude(scene => scene.SceneCharacters)
                    .ThenInclude(character => character.Spells)
                        .ThenInclude(link => link.PlaythroughSpell)
                            .ThenInclude(spell => spell.PlaythroughSpellType)
            .Include(playthrough => playthrough.Scenes)
                .ThenInclude(scene => scene.SceneCharacters)
                    .ThenInclude(character => character.ConsumableItems)
                        .ThenInclude(link => link.PlaythroughConsumableItem)
            .Include(playthrough => playthrough.Scenes)
                .ThenInclude(scene => scene.SceneCharacters)
                    .ThenInclude(character => character.EquippableItems)
                        .ThenInclude(link => link.PlaythroughEquippableItem)
                            .ThenInclude(item => item.AffectedSpellType)
            .Include(playthrough => playthrough.Scenes)
                .ThenInclude(scene => scene.SceneCharacters)
                    .ThenInclude(character => character.EquippableItems)
                        .ThenInclude(link => link.PlaythroughEquippableItem)
                            .ThenInclude(item => item.AddedSpells)
                                .ThenInclude(spell => spell.PlaythroughSpellType)
            .SingleOrDefaultAsync(ct);
    }

    public Task AddAsync(PlaythroughJoinSession session, CancellationToken ct)
    {
        return dbContext.PlaythroughJoinSessions.AddAsync(session, ct).AsTask();
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
