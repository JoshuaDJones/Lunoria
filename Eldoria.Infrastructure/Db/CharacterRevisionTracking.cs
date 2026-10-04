using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Eldoria.Infrastructure.Db;

// Centralized so every tracked authoring write uses the same revision rules.
internal static class CharacterRevisionTracking
{
    public static async Task PrepareAsync(ApplicationDbContext db, CancellationToken ct)
    {
        db.ChangeTracker.DetectChanges();
        // Type name/description/fallback artwork are shared spell data too.
        var changedTypeIds = db.ChangeTracker.Entries<SpellType>()
            .Where(e => e.State == EntityState.Modified &&
                Changed(e, nameof(SpellType.TypeName), nameof(SpellType.Description), nameof(SpellType.PhotoUrl)))
            .Select(e => e.Entity.Id).ToList();
        if (changedTypeIds.Count > 0)
        {
            var affectedSpells = await db.Spells.Where(spell => changedTypeIds.Contains(spell.SpellTypeId)).ToListAsync(ct);
            foreach (var spell in affectedSpells)
                spell.Revision = checked(db.Entry(spell).OriginalValues.GetValue<int>(nameof(Spell.Revision)) + 1);
        }
        foreach (var entry in db.ChangeTracker.Entries<Character>().Where(e => e.State == EntityState.Modified))
        {
            if (Changed(entry, nameof(Character.BaseMaxHp), nameof(Character.BaseMaxMp),
                nameof(Character.BaseMeleeAttackDamage), nameof(Character.BaseBowAttackDamage),
                nameof(Character.BaseMovement), nameof(Character.BaseMaxConsumableInventory),
                nameof(Character.BaseMaxEquippableInventory)))
                entry.Entity.StatsRevision = checked(entry.OriginalValues.GetValue<int>(nameof(Character.StatsRevision)) + 1);
            if (Changed(entry, nameof(Character.BaseAlternateFormId)))
                entry.Entity.AlternateFormRevision = checked(entry.OriginalValues.GetValue<int>(nameof(Character.AlternateFormRevision)) + 1);
        }
        foreach (var entry in db.ChangeTracker.Entries<Spell>().Where(e => e.State == EntityState.Modified))
        {
            if (Changed(entry, nameof(Spell.Name), nameof(Spell.Description), nameof(Spell.PhotoUrl),
                nameof(Spell.Range), nameof(Spell.IsRadius), nameof(Spell.MpCost),
                nameof(Spell.DamageEffect), nameof(Spell.HealthEffect), nameof(Spell.MagicEffect),
                nameof(Spell.SpellTypeId), nameof(Spell.IsDeleted)))
                entry.Entity.Revision = checked(entry.OriginalValues.GetValue<int>(nameof(Spell.Revision)) + 1);
        }
        var changedCharacterIds = db.ChangeTracker.Entries<CharacterSpell>()
            .Where(e => e.State is EntityState.Added or EntityState.Deleted)
            .Select(e => e.Entity.CharacterId).Where(id => id > 0).Distinct().ToList();
        foreach (var id in changedCharacterIds)
        {
            var character = await db.Characters.FindAsync([id], ct);
            if (character is not null)
                character.SpellAssignmentsRevision = checked(db.Entry(character).OriginalValues
                    .GetValue<int>(nameof(Character.SpellAssignmentsRevision)) + 1);
        }
        // New assignments have already seen their current shared spell definition.
        foreach (var entry in db.ChangeTracker.Entries<JourneyCharacterSpell>().Where(e => e.State == EntityState.Added).ToList())
        {
            if (entry.Entity.AcknowledgedSpellRevision is null)
                entry.Entity.AcknowledgedSpellRevision = (await db.Spells.FindAsync([entry.Entity.SpellId], ct))?.Revision;
        }
        foreach (var entry in db.ChangeTracker.Entries<SceneCharacterSpell>().Where(e => e.State == EntityState.Added).ToList())
        {
            if (entry.Entity.AcknowledgedSpellRevision is null)
                entry.Entity.AcknowledgedSpellRevision = (await db.Spells.FindAsync([entry.Entity.SpellId], ct))?.Revision;
        }
    }

    private static bool Changed(EntityEntry entry, params string[] properties) =>
        properties.Any(name => !Equals(entry.Property(name).OriginalValue, entry.Property(name).CurrentValue));
}
