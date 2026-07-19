namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughCharacterEquippableItemDto
    {
        public int Id { get; set; }
        public EquippableItemDto EquippableItem { get; set; } = null!;
        public JourneyPlaythroughCharacterDto JourneyPlaythroughCharacter { get; set; } = null!;
    }
}
