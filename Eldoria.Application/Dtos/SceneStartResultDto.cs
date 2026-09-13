namespace Eldoria.Application.Dtos;

public sealed class SceneStartResultDto
{
    public bool Started { get; set; }
    public SceneStartInventoryDto? PendingInventory { get; set; }
}

public sealed class SceneStartInventoryDto
{
    public Guid ResolutionToken { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string RewardName { get; set; } = string.Empty;
    public bool IsEquippable { get; set; }
    public int RemainingQuantity { get; set; }
    public int InventoryCount { get; set; }
    public int InventoryCapacity { get; set; }
    public List<SceneStartInventoryItemDto> Items { get; set; } = [];
    public List<int> RewardRecipientIds { get; set; } = [];
    public List<SceneStartRecipientDto> Recipients { get; set; } = [];
}

public sealed class SceneStartInventoryItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<int> RecipientIds { get; set; } = [];
}

public sealed class SceneStartRecipientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AvailableSlots { get; set; }
}

public sealed class SceneInventoryResolutionInput
{
    public Guid ResolutionToken { get; set; }
    // Null selects one incoming reward; otherwise this is a runtime inventory-link ID.
    public int? InventoryItemId { get; set; }
    // Null discards the selected item; otherwise transfer it to this runtime player.
    public int? TargetJourneyCharacterId { get; set; }
}
