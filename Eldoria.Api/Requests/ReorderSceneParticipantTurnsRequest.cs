using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class ReorderSceneParticipantTurnsRequest
    {
        [Required]
        [MinLength(1)]
        public List<SceneParticipantTurnPositionRequest> Turns { get; set; } = [];
    }

    public class SceneParticipantTurnPositionRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int? TurnId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? TurnPosition { get; set; }
    }
}
