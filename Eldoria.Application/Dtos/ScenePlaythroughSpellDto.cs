namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughSpellDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MpCost { get; set; }
    public int Range { get; set; }
    public bool IsRadius { get; set; }
    public int? DamageEffect { get; set; }
    public int? HealthEffect { get; set; }
    public int? MagicEffect { get; set; }
    public bool IsSupport { get; set; }
    public bool IsUtility { get; set; }
}
