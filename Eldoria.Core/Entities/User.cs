namespace Eldoria.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public ICollection<Series> Series { get; set; } = [];
        public ICollection<Journey> Journeys { get; set; } = [];
        public ICollection<Character> Characters { get; set; } = [];
        public ICollection<Item> Items { get; set; } = [];
        public ICollection<EquippableItem> EquippableItems { get; set; } = [];
        public ICollection<Spell> Spells { get; set; } = [];
        public ICollection<SpellType> SpellTypes { get; set; } = [];
    }
}
