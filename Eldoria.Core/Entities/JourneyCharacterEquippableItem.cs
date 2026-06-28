namespace Eldoria.Core.Entities
{
    public class JourneyCharacterEquippableItem
    {
        public int Id { get; set; }
        public bool IsEquipped { get; set; }

        public int JourneyCharacterId { get; set; }
        public JourneyCharacter JourneyCharacter { get; set; } = null!;

        public int EquippableItemId { get; set; }
        public EquippableItem EquippableItem { get; set; } = null!;
    }
}
