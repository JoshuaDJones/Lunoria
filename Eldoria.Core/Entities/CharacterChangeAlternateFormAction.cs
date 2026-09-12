namespace Eldoria.Core.Entities;

public class CharacterChangeAlternateFormAction
{
    public int Id { get; set; }
    public int? CharacterId { get; set; }
    public Character? Character { get; set; }
    public int AlternateFormId { get; set; }
    public Character AlternateForm { get; set; } = null!;
    public int SceneEventActionId { get; set; }
    public SceneEventAction SceneEventAction { get; set; } = null!;
}
