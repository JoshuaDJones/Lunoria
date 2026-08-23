namespace Eldoria.Application.Dtos;

public sealed class PublicPlaythroughSpellDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string SpellType { get; set; } = string.Empty;
    public int Range { get; set; }
    public bool IsRadius { get; set; }
    public int MpCost { get; set; }
    public int? DamageEffect { get; set; }
    public int? HealthEffect { get; set; }
    public int? MagicEffect { get; set; }
}
