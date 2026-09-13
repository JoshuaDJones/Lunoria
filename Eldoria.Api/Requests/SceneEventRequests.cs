using System.ComponentModel.DataAnnotations;
using Eldoria.Core.Enums;

namespace Eldoria.Api.Requests
{

    public class CreateSceneEventRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Description { get; set; }
    }

    public class UpdateSceneEventRequest : CreateSceneEventRequest;

    public class ReorderSceneEventsRequest
    {
        [Required, MinLength(1)]
        public List<SceneEventOrderRequest> Events { get; set; } = [];
    }

    public class ReorderSceneEventActionsRequest
    {
        [Required, MinLength(1)]
        public List<SceneEventOrderRequest> Actions { get; set; } = [];
    }

    public class SceneEventOrderRequest
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }
    }

    public class SceneEventActionRequest : IValidatableObject
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public ActionTargetType? ActionTargetType { get; set; }
        [Required]
        public EventActionType? EventActionType { get; set; }
        public CharacterStatType? CharacterStatType { get; set; }
        public AdjustmentOperation? AdjustmentOperation { get; set; }
        public int Value { get; set; }
        [Range(1, int.MaxValue)]
        public int? CharacterId { get; set; }
        [Range(1, int.MaxValue)]
        public int? AlternateFormId { get; set; }
        [Range(1, int.MaxValue)]
        public int? SpellId { get; set; }
        [Range(1, int.MaxValue)]
        public int? ConsumableItemId { get; set; }
        [Range(1, int.MaxValue)]
        public int? EquippableItemId { get; set; }
        [Range(1, 1000)]
        public int Quantity { get; set; } = 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EventActionType == Eldoria.Core.Enums.EventActionType.CharacterAddSpell && SpellId is null)
                yield return new ValidationResult("A spell is required.", [nameof(SpellId)]);
            if (EventActionType == Eldoria.Core.Enums.EventActionType.CharacterGiveItem &&
                ConsumableItemId.HasValue == EquippableItemId.HasValue)
                yield return new ValidationResult("Select exactly one consumable or equippable item.", [nameof(ConsumableItemId), nameof(EquippableItemId)]);
            if (EventActionType == Eldoria.Core.Enums.EventActionType.CharacterStatAdjustment)
            {
                if (CharacterStatType is null)
                    yield return new ValidationResult("A stat is required.", [nameof(CharacterStatType)]);
                if (AdjustmentOperation is null)
                    yield return new ValidationResult("An operation is required.", [nameof(AdjustmentOperation)]);
            }
            if (EventActionType == Eldoria.Core.Enums.EventActionType.CharacterChangeAlternateForm && AlternateFormId is null)
                yield return new ValidationResult("An alternate character is required.", [nameof(AlternateFormId)]);
        }
    }
}
