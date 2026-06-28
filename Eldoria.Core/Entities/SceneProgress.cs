using Eldoria.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Core.Entities
{
    public class SceneProgress
    {
        public int Id { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public int JourneyPlaythroughId { get; set; }
        public JourneyPlaythrough JourneyPlaythrough { get; set; } = null!;

        public SceneProgressStatus SceneProgressStatus { get; set; }
        public ICollection<SceneParticipant> Participants { get; set; } = [];
        public ICollection<SceneParticipantTurn> ParticipantTurns { get; set; } = [];
    }
}
