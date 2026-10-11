using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests;

public class SceneEventDraftRequest : CreateSceneEventRequest, IValidatableObject
{
    [Required, MinLength(1), MaxLength(100)]
    public List<SceneEventActionDraftRequest> Actions { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Actions?.Any(action => action is null) == true)
            yield return new ValidationResult("Actions cannot be null.", [nameof(Actions)]);
    }
}
