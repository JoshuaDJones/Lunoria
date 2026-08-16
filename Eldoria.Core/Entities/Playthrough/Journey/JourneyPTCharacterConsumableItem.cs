using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Journey
{
    public class JourneyPTCharacterConsumableItem
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }

        public int JourneyPTCharacterId { get; set; }
        public JourneyPTCharacter JourneyPTCharacter { get; set; } = null!;

        public int PlaythroughConsumableItemId { get; set; }
        public PlaythroughConsumableItem PlaythroughConsumableItem { get; set; } = null!;
    }
}
