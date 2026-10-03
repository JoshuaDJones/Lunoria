namespace Eldoria.Core.Entities.Playthrough.Scene;

public class ScenePTObjectivePoint
{
    public int Id { get; set; }
    public int ScenePTObjectiveId { get; set; }
    public ScenePTObjective SceneObjective { get; set; } = null!;
    public int SortOrder { get; set; }
    public string Text { get; set; } = string.Empty;
}
