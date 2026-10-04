using System.Security.Cryptography;
using System.Text.Json;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Common;

public static class CharacterSyncMappings
{
    public static IEnumerable<(Spell Spell, int? AcknowledgedRevision)> AssignedSpells(this ICharacterSyncTarget target) =>
        target switch
        {
            JourneyCharacter journey => journey.JourneyCharacterSpells.Select(link => (link.Spell, link.AcknowledgedSpellRevision)),
            SceneCharacter scene => scene.SceneCharacterSpells.Select(link => (link.Spell, link.AcknowledgedSpellRevision)),
            _ => throw new ArgumentException("Unsupported character assignment.", nameof(target))
        };

    public static CharacterSyncStatusDto SyncStatus(this ICharacterSyncTarget target)
    {
        var source = target.Character;

        return new(
            source is not null && !source.IsDeleted,
            new(source?.StatsRevision ?? 0, target.SyncedStatsRevision, target.AcknowledgedStatsRevision),
            new(source?.SpellAssignmentsRevision ?? 0, target.SyncedSpellAssignmentsRevision, target.AcknowledgedSpellAssignmentsRevision),
            new(source?.AlternateFormRevision ?? 0, target.SyncedAlternateFormRevision, target.AcknowledgedAlternateFormRevision),
            target.AssignedSpells().Where(link => link.Spell is not null).OrderBy(link => link.Spell.Id)
                .Select(link => new CharacterSpellUpdateDto(link.Spell.Id, link.Spell.Name, link.Spell.Revision,
                    link.AcknowledgedRevision, link.Spell.IsDeleted)).ToList());
    }

    public static CharacterSyncStatsDto Stats(this ICharacterSyncTarget target) => new(
        target.MaxHp, target.MaxMp, target.MeleeAttackDamage, target.BowAttackDamage, target.Movement,
        target.MaxConsumableInventory, target.MaxEquippableInventory);

    public static CharacterSyncStatsDto BaseStats(this Character source) => new(
        source.BaseMaxHp, source.BaseMaxMp, source.BaseMeleeAttackDamage, source.BaseBowAttackDamage, source.BaseMovement,
        source.BaseMaxConsumableInventory, source.BaseMaxEquippableInventory);

    public static CharacterSyncPreviewDto SyncPreview(this ICharacterSyncTarget target, Character source)
    {
        var preview = new CharacterSyncPreviewDto(
            target.Id, source.Id, target.SyncStatus(), target.Stats(), source.BaseStats(),
            target.AlternateFormId, source.BaseAlternateFormId,
            target.AssignedSpells().OrderBy(link => link.Spell.Id).Select(link => link.Spell.ToDto()).ToList(),
            source.CharacterSpells.Where(link => !link.Spell.IsDeleted).OrderBy(link => link.SpellId)
                .Select(link => link.Spell.ToDto()).ToList(),
            "", "Assigned spells reference shared definitions. Spell edits already apply here; acknowledging them does not restore old values. Existing playthroughs are unchanged.");
        var tokenData = JsonSerializer.SerializeToUtf8Bytes(new
        {
            Kind = target.GetType().Name,
            Preview = preview,
            target.RowVersion,
            SourceVersion = source.RowVersion,
            // Include revisions even for newly added source spells not currently assigned.
            SourceSpells = source.CharacterSpells.OrderBy(link => link.SpellId)
                .Select(link => new { link.SpellId, link.Spell.Revision, link.Spell.IsDeleted })
        });
        return preview with { ReviewToken = Convert.ToHexString(SHA256.HashData(tokenData)) };
    }

    public static void CopyStats(this ICharacterSyncTarget target, Character source)
    {
        target.MaxHp = source.BaseMaxHp;
        target.MaxMp = source.BaseMaxMp;
        target.MeleeAttackDamage = source.BaseMeleeAttackDamage;
        target.BowAttackDamage = source.BaseBowAttackDamage;
        target.Movement = source.BaseMovement;
        target.MaxConsumableInventory = source.BaseMaxConsumableInventory;
        target.MaxEquippableInventory = source.BaseMaxEquippableInventory;
        target.SyncedStatsRevision = source.StatsRevision;
    }
}
