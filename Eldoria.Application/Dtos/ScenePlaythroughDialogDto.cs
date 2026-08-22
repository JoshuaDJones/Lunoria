namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughDialogDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<ScenePlaythroughDialogPageDto> DialogPages { get; set; } = [];
}
