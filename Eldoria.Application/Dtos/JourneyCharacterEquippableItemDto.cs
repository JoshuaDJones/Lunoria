namespace Eldoria.Application.Dtos
{
    public class JourneyCharacterEquippableItemDto
    {
        public int Id { get; set; }
        public int JourneyCharacterId { get; set; }
        public int EquippableItemId { get; set; }
        public bool IsEquipped { get; set; }
        public EquippableItemDto EquippableItem { get; set; } = null!;
    }
}
