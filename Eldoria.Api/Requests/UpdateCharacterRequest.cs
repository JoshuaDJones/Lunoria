namespace Eldoria.Api.Requests
{
    public class UpdateCharacterRequest
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public IFormFile? Photo { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int? MeleeAttackDamage { get; set; }
        public int? BowAttackDamage { get; set; }
        public int Movement { get; set; }
        public int MaxInventory { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsNPC { get; set; }
        public bool IsEnemy { get; set; }
    }
}
