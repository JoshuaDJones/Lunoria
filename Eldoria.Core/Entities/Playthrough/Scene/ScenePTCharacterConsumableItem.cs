using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTCharacterConsumableItem
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }

        public int ScenePTCharacterId { get; set; }
        public ScenePTCharacter ScenePTCharacter { get; set; } = null!;

        public int PlaythroughConsumableItemId { get; set; }
        public PlaythroughConsumableItem PlaythroughConsumableItem { get; set; } = null!;
    }
}
