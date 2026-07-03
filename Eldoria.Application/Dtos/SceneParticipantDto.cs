namespace Eldoria.Application.Dtos
{
    public class SceneParticipantDto
    {
        public int Id { get; set; }
        public int SceneProgressId { get; set; }
        public int? JourneyCharacterId { get; set; }
        public int? SceneCharacterId { get; set; }
        public List<SceneParticipantTurnDto> Turns { get; set; } = [];
    }
}
