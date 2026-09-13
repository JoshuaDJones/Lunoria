namespace Eldoria.Application.Dtos;

public class SceneEventGrantInput
{
    public int? SpellId { get; set; }
    public int? ConsumableItemId { get; set; }
    public int? EquippableItemId { get; set; }
    public int Quantity { get; set; } = 1;
}
