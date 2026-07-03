namespace Eldoria.Application.Dtos
{
    public class SceneParticipantTurnDto
    {
        public int Id { get; set; }
        public int SceneProgressId { get; set; }
        public int SceneParticipantId { get; set; }
        public int TurnPosition { get; set; }
    }
}
