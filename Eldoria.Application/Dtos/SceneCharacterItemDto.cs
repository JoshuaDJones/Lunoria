namespace Eldoria.Application.Dtos
{
    public class SceneCharacterItemDto
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public int SceneCharacterId { get; set; }
        public int ItemId { get; set; }
        public ItemDto Item { get; set; } = null!;
    }
}
