namespace Eldoria.Core.Entities;

public class SceneObjectivePoint
{
    public int Id { get; set; }
    public int SceneObjectiveId { get; set; }
    public SceneObjective SceneObjective { get; set; } = null!;
    public int SortOrder { get; set; }
    public string Text { get; set; } = string.Empty;
}
