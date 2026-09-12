namespace Eldoria.Application.Dtos;

public class CharacterChangeAlternateFormActionDto
{
    public int? CharacterId { get; set; }
    public string? CharacterName { get; set; }
    public int AlternateFormId { get; set; }
    public string AlternateFormName { get; set; } = string.Empty;
}
