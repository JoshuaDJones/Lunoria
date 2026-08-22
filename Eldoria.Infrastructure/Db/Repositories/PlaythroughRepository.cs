using System.Data;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class PlaythroughRepository(ApplicationDbContext dbContext)
    : IPlaythroughRepository
{
    public async Task<IPlaythroughTransaction> BeginStartTransactionAsync(
        CancellationToken ct)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            ct);

        return new PlaythroughTransaction(transaction);
    }

    public async Task<PlaythroughStartAssets> GetStartAssetsAsync(
        int userId,
        IReadOnlyCollection<int> referencedCharacterIds,
        CancellationToken ct)
    {
        var allCharacters = await dbContext.Characters
            .IgnoreQueryFilters()
            .AsNoTrackingWithIdentityResolution()
            .Where(character => character.UserId == userId)
            .Include(character => character.CharacterDialogSettings)
            .Include(character => character.CharacterSpells)
            .ToListAsync(ct);

        var includedCharacterIds = allCharacters
            .Where(character => !character.IsDeleted)
            .Select(character => character.Id)
            .Concat(referencedCharacterIds)
            .ToHashSet();

        bool addedAlternateForm;

        do
        {
            addedAlternateForm = false;

            foreach (var character in allCharacters)
            {
                if (!includedCharacterIds.Contains(character.Id) ||
                    character.BaseAlternateFormId is not int alternateFormId)
                {
                    continue;
                }

                addedAlternateForm |= includedCharacterIds.Add(alternateFormId);
            }
        }
        while (addedAlternateForm);

        var characters = allCharacters
            .Where(character => includedCharacterIds.Contains(character.Id))
            .ToList();

        var consumables = await dbContext.ConsumableItems
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .ToListAsync(ct);

        var equippables = await dbContext.EquippableItems
            .AsNoTrackingWithIdentityResolution()
            .Where(item => item.UserId == userId)
            .Include(item => item.AddedSpells)
            .Include(item => item.AffectedSpellType)
            .ToListAsync(ct);

        var spells = await dbContext.Spells
            .AsNoTracking()
            .Where(spell => spell.UserId == userId)
            .ToListAsync(ct);

        var spellTypes = await dbContext.SpellTypes
            .AsNoTracking()
            .Where(spellType => spellType.UserId == userId)
            .ToListAsync(ct);

        return new PlaythroughStartAssets
        {
            Characters = characters,
            Consumables = consumables,
            Equippables = equippables,
            Spells = spells,
            SpellTypes = spellTypes
        };
    }

    public Task<List<Playthrough>> ListUnfinishedForJourneyAsync(
        int userId,
        int sourceJourneyId,
        CancellationToken ct)
    {
        return dbContext.Playthroughs
            .Where(playthrough =>
                playthrough.UserId == userId &&
                playthrough.SourceJourneyId == sourceJourneyId &&
                playthrough.CompletedAt == null)
            .ToListAsync(ct);
    }

    public Task<List<Playthrough>> ListForJourneyAsync(
        int userId,
        int sourceJourneyId,
        CancellationToken ct)
    {
        return dbContext.Playthroughs
            .AsNoTracking()
            .Where(playthrough =>
                playthrough.UserId == userId &&
                playthrough.SourceJourneyId == sourceJourneyId)
            .OrderBy(playthrough => playthrough.CompletedAt != null)
            .ThenByDescending(playthrough => playthrough.StartedAt)
            .ToListAsync(ct);
    }

    public Task AddAsync(Playthrough playthrough, CancellationToken ct)
    {
        return dbContext.Playthroughs.AddAsync(playthrough, ct).AsTask();
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return dbContext.SaveChangesAsync(ct);
    }

    private sealed class PlaythroughTransaction(IDbContextTransaction transaction)
        : IPlaythroughTransaction
    {
        public Task CommitAsync(CancellationToken ct)
        {
            return transaction.CommitAsync(ct);
        }

        public ValueTask DisposeAsync()
        {
            return transaction.DisposeAsync();
        }
    }
}
