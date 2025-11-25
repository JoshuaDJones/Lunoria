namespace Eldoria.BlazorClient.Dtos
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int HpEffect { get; set; }
        public int MpEffect { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
