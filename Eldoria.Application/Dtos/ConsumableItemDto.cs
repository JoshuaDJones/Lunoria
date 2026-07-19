namespace Eldoria.Application.Dtos
{
    public class ConsumableItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int HpEffect { get; set; }
        public int MpEffect { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
