namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughCharacterConsumableItemDto
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public ConsumableItemDto ConsumableItem { get; set; } = null!;
        public JourneyPlaythroughCharacterDto JourneyPlaythroughCharacter { get; set; } = null!;
    }
}
