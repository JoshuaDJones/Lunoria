using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTCharacterEquippableItem
    {
        public int Id { get; set; }
        public bool IsEquipped { get; set; }

        public int ScenePTCharacterId { get; set; }
        public ScenePTCharacter ScenePTCharacter { get; set; } = null!;

        public int PlaythroughEquippableItemId { get; set; }
        public PlaythroughEquippableItem PlaythroughEquippableItem { get; set; } = null!;
    }
}
