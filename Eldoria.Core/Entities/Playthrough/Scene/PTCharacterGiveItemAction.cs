using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene;

public class PTCharacterGiveItemAction
{
    public int Id { get; set; }
    public int SourceCharacterGiveItemActionId { get; set; }
    public int? PlaythroughCharacterId { get; set; }
    public PlaythroughCharacter? PlaythroughCharacter { get; set; }
    public int? PlaythroughConsumableItemId { get; set; }
    public PlaythroughConsumableItem? PlaythroughConsumableItem { get; set; }
    public int? PlaythroughEquippableItemId { get; set; }
    public PlaythroughEquippableItem? PlaythroughEquippableItem { get; set; }
    public int Quantity { get; set; } = 1;
    public int ScenePTActionEventId { get; set; }
    public ScenePTActionEvent ScenePTActionEvent { get; set; } = null!;
}
