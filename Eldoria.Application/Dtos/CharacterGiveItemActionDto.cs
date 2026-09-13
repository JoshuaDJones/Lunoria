namespace Eldoria.Application.Dtos;

public class CharacterGiveItemActionDto
{
    public int? CharacterId { get; set; }
    public string? CharacterName { get; set; }
    public int? ConsumableItemId { get; set; }
    public int? EquippableItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
