namespace Eldoria.Application.Dtos
{
    public class SceneEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public SceneDto Scene { get; set; } = null!;
        public ICollection<SceneEventActionDto> SceneEventActions { get; set; } = [];
    }
}
