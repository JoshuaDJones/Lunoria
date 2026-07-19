using Eldoria.Core.Enums;

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
        public int BaseMaxConsumableInventory { get; set; }
        public int BaseMaxEquippableInventory { get; set; }
        public CharacterType CharacterType { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? AlternateFormId { get; set; }
        public CharacterDto? AlternateForm { get; set; }
        public List<CharacterSpellDto>? CharacterSpells { get; set; } = [];
        public CharacterDialogSettingsDto? CharacterDialogSettings { get; set; }
    }
}
