namespace Eldoria.Application.Dtos;

public sealed class SceneAttackResultDto
{
    public int Damage { get; set; }
    public bool IsSupport { get; set; }
    public bool IsUtility { get; set; }
    public int HealthRestored { get; set; }
    public int MagicRestored { get; set; }
    public int TargetCurrentHp { get; set; }
    public bool TargetDefeated { get; set; }
    public string? RewardStat { get; set; }
    public int RewardAmount { get; set; }
}
