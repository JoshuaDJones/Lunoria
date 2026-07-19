namespace Eldoria.Core.Entities
{
    public class JourneyPlaythroughCharacterEquippableItem
    {
        public int Id { get; set; }

        public int EquippableItemId { get; set; }
        public EquippableItem EquippableItem { get; set; } = null!;

        public int JourneyPlaythroughCharacterId { get; set; }
        public JourneyPlaythroughCharacter JourneyPlaythroughCharacter { get; set; } = null!;
    }
}
