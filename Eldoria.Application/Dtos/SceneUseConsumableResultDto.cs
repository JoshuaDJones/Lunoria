namespace Eldoria.Application.Dtos;

public sealed class SceneUseConsumableResultDto
{
    public int InventoryItemId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int HpRestored { get; set; }
    public int MpRestored { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMp { get; set; }
    public int MaxMp { get; set; }
}
