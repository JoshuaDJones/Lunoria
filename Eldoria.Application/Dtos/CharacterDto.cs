using Eldoria.Core.Entities;

namespace Eldoria.Application.Dtos
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }
        public int Movement { get; set; }
        public int MaxInventory { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsNPC { get; set; }
        public bool IsEnemy { get; set; }
        public DateTime CreateDate { get; set; }

        public int? AlternateFormId { get; set; }
        public CharacterDto? AlternateForm { get; set; }
        public List<CharacterSpellDto>? CharacterSpells { get; set; } = [];
    }
}
