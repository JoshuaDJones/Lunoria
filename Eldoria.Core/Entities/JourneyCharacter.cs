namespace Eldoria.Core.Entities
{
    public class JourneyCharacter
    {
        public int Id { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public bool IsDown { get; set; }
        public bool IsAlternateForm { get; set; }

        public int JourneyId { get; set; }
        public Journey Journey { get; set; } = null!;

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public ICollection<JourneyCharacterItem> JourneyCharacterItems { get; set; } = [];
    }
}
