namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class PlaythroughConsumableItem
    {
        public int Id { get; set; }
        public int SourceConsumableItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int HpEffect { get; set; }
        public int MpEffect { get; set; }

        public int PlaythroughId { get; set; }
        public Playthrough Playthrough { get; set; } = null!;
    }
}
