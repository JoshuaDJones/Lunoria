namespace Eldoria.Application.Dtos;

public class CharacterAddSpellActionDto
{
    public int? CharacterId { get; set; }
    public string? CharacterName { get; set; }
    public int SpellId { get; set; }
    public string SpellName { get; set; } = string.Empty;
}
