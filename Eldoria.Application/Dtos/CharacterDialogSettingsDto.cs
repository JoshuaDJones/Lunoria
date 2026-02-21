namespace Eldoria.Application.Dtos
{
    public class CharacterDialogSettingsDto
    {
        public int Id { get; set; }

        public string DialogActiveColor { get; set; } = string.Empty;
        public string DialogUnActiveColor { get; set; } = string.Empty;
    }
}
