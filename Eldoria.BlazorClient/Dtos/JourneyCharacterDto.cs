namespace Eldoria.BlazorClient.Dtos
{
    public class JourneyCharacterDto
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int CharacterId { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public bool IsDown { get; set; }
        public bool IsAlternateForm { get; set; }    
        public CharacterDto Character { get; set; } = null!;
        public List<JourneyCharacterItemDto> JourneyCharacterItems { get; set; } = [];
    }
}
