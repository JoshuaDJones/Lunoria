using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public class SceneEventActionDraftInput
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ActionTargetType ActionTargetType { get; set; }
    public EventActionType EventActionType { get; set; }
    public CharacterStatType CharacterStatType { get; set; } = CharacterStatType.CurrentHp;
    public AdjustmentOperation AdjustmentOperation { get; set; } = AdjustmentOperation.Set;
    public int Value { get; set; }
    public int? CharacterId { get; set; }
    public int? AlternateFormId { get; set; }
    public SceneEventGrantInput Grant { get; set; } = new();
}
