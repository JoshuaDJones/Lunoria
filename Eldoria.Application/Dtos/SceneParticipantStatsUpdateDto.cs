namespace Eldoria.Application.Dtos;

public sealed class SceneParticipantStatsUpdateDto
{
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMp { get; set; }
    public int MaxMp { get; set; }
    public int Movement { get; set; }
    public int? MeleeAttackDamage { get; set; }
    public int? BowAttackDamage { get; set; }
}
