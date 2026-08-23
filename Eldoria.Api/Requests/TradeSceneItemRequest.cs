namespace Eldoria.Api.Requests;

public sealed class TradeSceneItemRequest
{
    public int TargetParticipantId { get; set; }
    public int InventoryItemId { get; set; }
    public bool IsEquippable { get; set; }
}
