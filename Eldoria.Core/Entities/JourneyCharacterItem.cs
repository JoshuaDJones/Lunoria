namespace Eldoria.Core.Entities
{
    public class JourneyCharacterItem
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }

        public int JourneyCharacterId { get; set; }
        public JourneyCharacter JourneyCharacter{ get; set; } = null!;

        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;
    }
}
