namespace Eldoria.BlazorClient.Dtos
{
    public class SceneCharacterDto
    {
        public int Id { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public bool IsDown { get; set; }
        public bool IsAlternateForm { get; set; }
        public int SceneId { get; set; }
        public int CharacterId { get; set; }
        public CharacterDto Character { get; set; } = null!;
        public List<SceneCharacterItemDto> SceneCharacterItems { get; set; } = [];

    }
}
