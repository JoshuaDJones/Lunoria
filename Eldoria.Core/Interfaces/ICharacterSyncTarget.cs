using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces;

public interface ICharacterSyncTarget
{
    int Id { get; }
    int CharacterId { get; }
    Character Character { get; }
    int MaxHp { get; set; }
    int MaxMp { get; set; }
    int? MeleeAttackDamage { get; set; }
    int? BowAttackDamage { get; set; }
    int Movement { get; set; }
    int MaxConsumableInventory { get; set; }
    int MaxEquippableInventory { get; set; }
    int? AlternateFormId { get; set; }
    Character? AlternateForm { get; set; }
    int? SyncedStatsRevision { get; set; }
    int? AcknowledgedStatsRevision { get; set; }
    int? SyncedSpellAssignmentsRevision { get; set; }
    int? AcknowledgedSpellAssignmentsRevision { get; set; }
    int? SyncedAlternateFormRevision { get; set; }
    int? AcknowledgedAlternateFormRevision { get; set; }
    byte[] RowVersion { get; }
}
