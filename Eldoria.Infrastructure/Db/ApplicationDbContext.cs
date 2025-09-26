using Eldoria.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterSpell> CharacterSpells { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Journey> Journeys { get; set; }
        public DbSet<JourneyCharacter> JourneyCharacters { get; set; }
        public DbSet<JourneyCharacterItem> JourneyCharacterItems { get; set; }
        public DbSet<Scene> Scenes { get; set; }
        public DbSet<SceneCharacter> SceneCharacters { get; set; }
        public DbSet<SceneCharacterItem> SceneCharacterItems { get; set; }
        public DbSet<SceneDialog> SceneDialogs { get; set; }
        public DbSet<Spell> Spells { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
