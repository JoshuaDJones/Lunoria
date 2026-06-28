using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Core.Entities
{
    public class SpellType
    {
        public int Id { get; set; }
        public required string TypeName { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Spell> Spells { get; set; } = [];
        public ICollection<EquippableItem> AffectedEquippableItems { get; set; } = [];
    }
}
