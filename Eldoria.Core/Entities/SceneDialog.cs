using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Core.Entities
{
    public class SceneDialog
    {
        public int Id { get; set; }
        public int OrderNum { get; set; }
        public string? PhotoUrl { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public string Dialog { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;
    }
}
