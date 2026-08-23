namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughSpellDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MpCost { get; set; }
    public int? DamageEffect { get; set; }
}
