namespace Eldoria.Application.Dtos
{
    public class JourneyCharacterItemDto
    {
        public int Id { get; set; }
        public int JourneyCharacterId { get; set; }
        public bool IsUsed { get; set; }
        public int ItemId { get; set; }
        public ItemDto Item { get; set; } = null!;
    }
}
