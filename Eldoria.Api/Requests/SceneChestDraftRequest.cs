using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests;

public class SceneChestDraftRequest : IValidatableObject
{
    [Required, MaxLength(250)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(1), MaxLength(6)]
    public List<SceneChestLootEntryRequest> LootEntries { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LootEntries?.Any(entry => entry is null) == true)
            yield return new ValidationResult("Loot entries cannot be null.", [nameof(LootEntries)]);
    }
}
