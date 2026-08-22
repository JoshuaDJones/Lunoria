namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughDialogSectionDto
{
    public int Id { get; set; }
    public int OrderNum { get; set; }
    public string ReadingText { get; set; } = string.Empty;
    public bool IsNarrator { get; set; }
    public ScenePlaythroughDialogCharacterDto? Character { get; set; }
}
