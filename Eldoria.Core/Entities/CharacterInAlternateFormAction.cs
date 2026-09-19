namespace Eldoria.Core.Entities;

public class CharacterInAlternateFormAction
{
    public int Id { get; set; }
    public int? CharacterId { get; set; }
    public Character? Character { get; set; }
    public int SceneEventActionId { get; set; }
    public SceneEventAction SceneEventAction { get; set; } = null!;
}
