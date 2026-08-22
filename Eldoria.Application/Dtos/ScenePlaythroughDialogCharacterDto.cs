namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughDialogCharacterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? PortraitUrl { get; set; }
    public string DialogActiveColor { get; set; } = string.Empty;
    public string DialogInActiveColor { get; set; } = string.Empty;
}
