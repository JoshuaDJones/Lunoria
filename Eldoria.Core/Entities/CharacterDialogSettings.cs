namespace Eldoria.Core.Entities
{
    public class CharacterDialogSettings
    {
        public const string DefaultActiveColor = "#808080";
        public const string DefaultInactiveColor = "#808080";

        public int Id { get; set; }

        public string DialogActiveColor { get; set; } = "#808080";
        public string DialogInActiveColor { get; set; } = "#808080";

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;
    }
}
