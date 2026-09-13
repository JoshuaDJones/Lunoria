namespace Eldoria.Core.Entities;

public class CharacterGiveItemAction
{
    public int Id { get; set; }
    public int? CharacterId { get; set; }
    public Character? Character { get; set; }
    public int? ConsumableItemId { get; set; }
    public ConsumableItem? ConsumableItem { get; set; }
    public int? EquippableItemId { get; set; }
    public EquippableItem? EquippableItem { get; set; }
    public int Quantity { get; set; } = 1;
    public int SceneEventActionId { get; set; }
    public SceneEventAction SceneEventAction { get; set; } = null!;
}
