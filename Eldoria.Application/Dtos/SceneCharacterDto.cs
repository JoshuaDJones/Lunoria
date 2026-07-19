namespace Eldoria.Application.Dtos
{
    public class SceneCharacterDto
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
        public CharacterDto? AlternateForm { get; set; }
        public int CharacterId { get; set; }
        public CharacterDto Character { get; set; } = null!;
        public List<SceneCharacterSpellDto> SceneCharacterSpells { get; set; } = [];

    }
}
