namespace Eldoria.Core.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Portrait { get; set; } = string.Empty;
        public string PortraitFileName { get; set; } = string.Empty;

        public int BaseMaxHp { get; set; }
        public int BaseMaxMp { get; set; }
        public int? BaseMeleeAttackDamage { get; set; }
        public int? BaseBowAttackDamage { get; set; }
        public int BaseMovement { get; set; }
        public int BaseMaxConsumableInventory { get; set; }
        public int BaseMaxEquippableInventory { get; set; }

        public bool IsPlayer { get; set; }
        public bool IsNPC { get; set; }
        public bool IsEnemy { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? BaseAlternateFormId { get; set; }
        public Character? BaseAlternateForm { get; set; } = null!;

        public CharacterDialogSettings CharacterDialogSettings { get; set; } = null!;

        public ICollection<CharacterSpell> CharacterSpells { get; set; } = [];
        public ICollection<DialogPageSection> DialogPageSections { get; set; } = [];

    }
}
