using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Journey
{
    public class JourneyPTCharacterEquippableItem
    {
        public int Id { get; set; }
        public bool IsEquipped { get; set; }

        public int JourneyPTCharacterId { get; set; }
        public JourneyPTCharacter JourneyPTCharacter { get; set; } = null!;

        public int PlaythroughEquippableItemId { get; set; }
        public PlaythroughEquippableItem PlaythroughEquippableItem { get; set; } = null!;
    }
}
