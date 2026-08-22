namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughDialogPageDto
{
    public int Id { get; set; }
    public int OrderNum { get; set; }
    public string? PhotoUrl { get; set; }
    public List<ScenePlaythroughDialogSectionDto> DialogPageSections { get; set; } = [];
}
