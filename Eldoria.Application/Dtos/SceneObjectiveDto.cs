namespace Eldoria.Application.Dtos;

public sealed class SceneObjectiveDto
{
    public int Id { get; set; }
    public int SortOrder { get; set; }
    public List<string> Points { get; set; } = [];
}
