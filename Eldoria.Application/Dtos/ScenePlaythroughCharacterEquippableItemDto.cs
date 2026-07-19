namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughCharacterEquippableItemDto
    {
        public int Id { get; set; }
        public EquippableItemDto EquippableItem { get; set; } = null!;
        public ScenePlaythroughCharacterDto ScenePlaythroughCharacter { get; set; } = null!;
    }
}
