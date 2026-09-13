using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene;

public class PTCharacterChangeAlternateFormAction
{
    public int Id { get; set; }
    public int SourceCharacterChangeAlternateFormActionId { get; set; }
    public int? PlaythroughCharacterId { get; set; }
    public PlaythroughCharacter? PlaythroughCharacter { get; set; }
    public int? AlternateFormId { get; set; }
    public PlaythroughCharacter? AlternateForm { get; set; }
    public int ScenePTActionEventId { get; set; }
    public ScenePTActionEvent ScenePTActionEvent { get; set; } = null!;
}
