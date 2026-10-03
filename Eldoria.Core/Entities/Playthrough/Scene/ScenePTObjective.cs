namespace Eldoria.Core.Entities.Playthrough.Scene;

public class ScenePTObjective
{
    public int Id { get; set; }
    public int ScenePTId { get; set; }
    public ScenePT Scene { get; set; } = null!;
    public int SortOrder { get; set; }
    public ICollection<ScenePTObjectivePoint> Points { get; set; } = [];
}
