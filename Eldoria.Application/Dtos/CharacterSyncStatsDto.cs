namespace Eldoria.Application.Dtos;

public sealed record CharacterSyncStatsDto(
    int MaxHp, int MaxMp, int? MeleeAttackDamage, int? BowAttackDamage, int Movement,
    int MaxConsumableInventory, int MaxEquippableInventory);

