using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Core.Entities
{
    public class SceneParticipantTurn
    {
        public int Id { get; set; }

        public int SceneProgressId { get; set; }
        public SceneProgress SceneProgress { get; set; } = null!;

        public int SceneParticipantId { get; set; }
        public SceneParticipant SceneParticipant { get; set; } = null!;

        public int TurnPosition { get; set; }
    }
}
