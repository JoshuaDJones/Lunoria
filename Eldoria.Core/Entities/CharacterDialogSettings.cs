namespace Eldoria.Core.Entities
{
    public class CharacterDialogSettings
    {
        public int Id { get; set; }

        public string DialogActiveColor { get; set; } = string.Empty;
        public string DialogUnActiveColor { get; set; } = string.Empty;

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;
    }
}
