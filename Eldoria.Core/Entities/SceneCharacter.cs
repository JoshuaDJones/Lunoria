namespace Eldoria.Core.Entities
{
    public class SceneCharacter
    {
        public int Id { get; set; }

        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }
        public int Movement { get; set; }
        public int MaxConsumableInventory { get; set; }
        public int MaxEquippableInventory { get; set; }

        public int MaxHp { get; set; }
        public int MaxMp { get; set; }

        public bool IsInitiallyActive { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; } = null!;

        public int? AlternateFormId { get; set; }
        public Character? AlternateForm { get; set; } = null!;

        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public ICollection<SceneCharacterSpell> SceneCharacterSpells { get; set; } = [];
    }
}
