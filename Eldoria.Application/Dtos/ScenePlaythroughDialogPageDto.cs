using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughDialogPageDto
{
    public int Id { get; set; }
    public int OrderNum { get; set; }
    public DialogPageType PageType { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string MediaContentType { get; set; } = string.Empty;
    public List<ScenePlaythroughDialogSectionDto> DialogPageSections { get; set; } = [];
}
