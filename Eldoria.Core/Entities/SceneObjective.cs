namespace Eldoria.Core.Entities;

public class SceneObjective
{
    public int Id { get; set; }
    public int SceneId { get; set; }
    public Scene Scene { get; set; } = null!;
    public int SortOrder { get; set; }
    public ICollection<SceneObjectivePoint> Points { get; set; } = [];
}
