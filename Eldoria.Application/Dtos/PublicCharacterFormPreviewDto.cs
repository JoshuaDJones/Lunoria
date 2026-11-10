namespace Eldoria.Application.Dtos;

public sealed class PublicCharacterFormPreviewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? PortraitUrl { get; set; }
    public int Movement { get; set; }
    public int? MeleeAttackDamage { get; set; }
    public int? BowAttackDamage { get; set; }
    public List<PublicPlaythroughSpellDto> Spells { get; set; } = [];
}
