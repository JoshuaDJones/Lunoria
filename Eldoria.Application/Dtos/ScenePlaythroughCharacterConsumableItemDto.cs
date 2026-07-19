namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughCharacterConsumableItemDto
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public ConsumableItemDto ConsumableItem { get; set; } = null!;
        public ScenePlaythroughCharacterDto ScenePlaythroughCharacter { get; set; } = null!;
    }
}
